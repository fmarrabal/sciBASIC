﻿#Region "Microsoft.VisualBasic::74a48bfd98dac8489458e34fc18652cd, ..\sciBASIC#\Data_science\Microsoft.VisualBasic.DataMining.Model.Network\BinaryTree\Tree.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace KMeans

    ''' <summary>
    ''' KMeans.Tree.NET
    ''' </summary>
    <PackageNamespace("KMeans.Tree.NET",
                      Category:=APICategories.ResearchTools,
                      Publisher:="smrucc@gcmodeller.org")>
    Public Module Tree

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="cluster"></param>
        ''' <param name="depth">将会以最短的聚类作为数据分区的深度</param>
        ''' <returns></returns>
        <Extension>
        Public Function Partitioning(cluster As IEnumerable(Of EntityLDM), Optional depth As Integer = -1, Optional trim As Boolean = True) As List(Of Partition)
            Dim list As New List(Of EntityLDM)(cluster)

            If depth <= 0 Then
                depth = (From x As EntityLDM
                         In list
                         Select l = x.Cluster
                         Order By l.Length Ascending).First.Split("."c).Length
            End If

            Dim clusters As New List(Of String)({""})

            For i As Integer = 0 To depth - 1
                Dim temp As New List(Of String)

                For Each x As String In clusters
                    temp += x & ".1"
                    temp += x & ".2"
                Next

                clusters = temp
            Next

            For i As Integer = 0 To clusters.Count - 1   ' 去掉最开始的小数点
                clusters(i) = Mid(clusters(i), 2)
            Next

            Dim partitions As New List(Of Partition)

            For Each tag As String In clusters
                Dim LQuery As EntityLDM() =
                    LinqAPI.Exec(Of EntityLDM) <=
 _
                    From x As EntityLDM
                    In list.AsParallel
                    Where InStr(x.Cluster, tag, CompareMethod.Binary) = 1
                    Select x

                list -= LQuery
                partitions += New Partition With {
                    .Tag = tag,
                    .uids = LQuery.ToArray(Function(x) x.Name),
                    .members = LQuery
                }
            Next

            If Not list.IsNullOrEmpty Then
                partitions += New Partition With {
                    .Tag = "Unclass",
                    .uids = list.ToArray(Function(x) x.Name),
                    .members = list.ToArray
                }
            End If

            If trim Then
                partitions = New List(Of Partition)(
                    partitions.Where(Function(x) x.NumOfEntity > 0))
            End If

            Return partitions
        End Function

        ''' <summary>
        ''' 二叉树树形聚类，请注意，所输入的数据的名字不可以一样，不然无法正确生成cluster标记
        ''' </summary>
        ''' <param name="resultSet"></param>
        ''' <param name="stop">Max iteration number for the kmeans kernel</param>
        ''' <returns></returns>
        '''
        <ExportAPI("Cluster.Trees")>
        <Extension> Public Function TreeCluster(resultSet As IEnumerable(Of EntityLDM), Optional parallel As Boolean = False, Optional [stop] As Integer = -1) As EntityLDM()
            Dim source As EntityLDM() = resultSet.ToArray
            Dim mapNames As String() = source(Scan0).Properties.Keys.ToArray   ' 得到所有属性的名称
            Dim ds As Entity() = source.ToArray(
                Function(x) New KMeans.Entity With {
                    .uid = x.Name,
                    .Properties = mapNames.ToArray(Function(s) x.Properties(s))
                })  ' 在这里生成计算模型
            Dim tree As KMeans.Entity() = TreeCluster(ds, parallel, [stop])   ' 二叉树聚类操作
            Dim saveResult As EntityLDM() = tree.ToArray(Function(x) x.ToLDM(mapNames))   ' 重新生成回数据模型

            For Each name As String In source.Select(Function(x) x.Name)
                For Each x In saveResult
                    If InStr(x.Name, name) > 0 Then
                        x.Cluster = x.Name.Replace(name & ".", "")
                        x.Name = name
                        Exit For
                    End If
                Next
            Next

            Return saveResult
        End Function

        ''' <summary>
        ''' 二叉树聚类的路径会在<see cref="Entity.uid"/>上面出现
        ''' </summary>
        ''' <param name="source">函数会在这里自动调用ToArray方法结束Linq查询</param>
        ''' <param name="parallel"></param>
        ''' <param name="stop">Max iteration number for the kmeans kernel</param>
        ''' <returns></returns>
        <ExportAPI("Cluster.Trees")>
        <Extension> Public Function TreeCluster(source As IEnumerable(Of Entity), Optional parallel As Boolean = False, Optional [stop] As Integer = -1) As Entity()
            Return TreeCluster(Of Entity)(source.ToArray, parallel, [stop])
        End Function

        Public Function TreeCluster(Of T As Entity)(source As IEnumerable(Of T), Optional parallel As Boolean = False, Optional [stop] As Integer = -1) As Entity()
            If parallel Then
                Return __firstCluster(source, [stop]:=CInt(source.Count / 2), kmeansStop:=[stop])
            Else
                Return __treeCluster(source, Scan0, CInt(source.Count / 2), kmeansStop:=[stop])
            End If
        End Function

        ''' <summary>
        ''' 两条线程并行化进行二叉树聚类
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="[stop]"></param>
        ''' <returns></returns>
        Private Function __firstCluster(Of T As Entity)(source As IEnumerable(Of T), [stop] As Integer, kmeansStop As Integer) As Entity()
            Dim result As KMeansCluster(Of T)() = ClusterDataSet(2, source, debug:=True, [stop]:=kmeansStop).ToArray
            ' 假设在刚开始不会出现为零的情况
            Dim cluster1 As AsyncHandle(Of Entity()) =
                New AsyncHandle(Of Entity())(Function() __rootCluster(result(0), "1", [stop], kmeansStop)).Run    ' cluster1
            Dim list As List(Of Entity) = New List(Of Entity) + __rootCluster(result(1), "2", [stop], kmeansStop) ' cluster2
            list += cluster1.GetValue

            Return list.ToArray
        End Function

        Private Function __rootCluster(Of T As Entity)(cluster As KMeans.KMeansCluster(Of T), id As String, [stop] As Integer, kmeansStop As Integer) As Entity()
            For Each x In cluster
                x.uid &= ("." & id)
            Next

            If cluster.NumOfEntity <= 1 Then
                Return cluster.ToArray
            Else
                Return __treeCluster(cluster.ToArray, Scan0, [stop], kmeansStop)  ' 递归聚类分解
            End If
        End Function

        Private Function __treeCluster(Of T As Entity)(source As IEnumerable(Of T), depth As Integer, [stop] As Integer, kmeansStop As Integer) As Entity()
            If source.Count = 2 Then
EXIT_:          Dim array = source.ToArray
                For i As Integer = 0 To array.Length - 1
                    Dim id As String = "." & CStr(i + 1)
                    array(i).uid &= id
                Next
                Return array
            Else
                depth += 1

                If depth >= [stop] Then
                    Dim cluster As T() = source.ToArray

                    For Each x As T In cluster
                        x.uid &= ".X"
                    Next

                    Return cluster
                End If
            End If

            Dim list As New List(Of Entity)
            Dim result As KMeansCluster(Of T)() = ClusterDataSet(2, source,, [stop]:=kmeansStop).ToArray

            ' 检查数据
            Dim b0 As Boolean = False ', b20 As Boolean = False

            For Each x In result
                If x.NumOfEntity = 0 Then
                    b0 = True
                    'Else
                    '    Dim nl As Integer = 0.75 * source.First.Properties.Count
                    '    If (From c In x.ClusterMean Where c = 0R Select 1).Count >= nl AndAlso
                    '        (From c In x.ClusterSum Where c = 0R Select 1).Count >= nl Then
                    '        b20 = True
                    '    End If
                End If
            Next

            If b0 Then    ' 已经无法再分了，全都是0，则放在一个cluster里面
                'Dim cluster As T() = result.MatrixToVector
                'For Each x In cluster
                '    x.uid &= ".X"
                'Next

                'Call list.Add(cluster)
                Call Console.Write(">")
                Call list.Add(__treeCluster(result.Unlist, depth, [stop], kmeansStop))  ' 递归聚类分解
            Else
                For i As Integer = 0 To result.Length - 1
                    Dim cluster = result(i)
                    Dim id As String = "." & CStr(i + 1)

                    For Each x In cluster
                        x.uid &= id
                    Next

                    If cluster.NumOfEntity = 1 Then
                        Call list.Add(cluster.Item(Scan0))
                    ElseIf cluster.NumOfEntity = 0 Then
                        '  不可以取消这可分支，否则会死循环
                    Else
                        Call Console.Write(">")
                        Call list.Add(__treeCluster(cluster.ToArray, depth, [stop], kmeansStop))  ' 递归聚类分解
                    End If
                Next
            End If

            Call Console.Write("<")

            Return list.ToArray
        End Function

        Private Structure __edgePath
            Public path As String()
            Public node As EntityLDM

            Public Overrides Function ToString() As String
                Return $"[{node.Cluster}] --> {node.Name}"
            End Function
        End Structure

        ''' <summary>
        ''' Create network model for visualize the binary tree clustering result.
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        <ExportAPI("Cluster.Trees.Network",
                   Info:="Create network model for visualize the binary tree clustering result.")>
        <Extension> Public Function bTreeNET(source As IEnumerable(Of EntityLDM)) As FileStream.Network
            Dim array = (From x As EntityLDM In source
                         Let path As String() = x.Cluster.Split("."c)
                         Select New __edgePath With {
                             .node = x,
                             .path = path}).ToArray
            Dim nodes = array.ToArray(Function(x) New FileStream.Node With {
                                          .ID = x.node.Name,
                                          .NodeType = "Entity",
                                          .Properties = x.node.Properties _
                                                .ToDictionary(Function(xx) xx.Key,
                                                              Function(xx) Math.Round(xx.Value, 4).ToString)
                                          }).ToList
            nodes += New FileStream.Node With {
                .ID = "ROOT",
                .NodeType = "ROOT"
            }

            Dim edges = __buildNET(array, nodes ^ Function(x As Node) String.Equals(x.NodeType, "ROOT"), Scan0, nodes)

            Return New FileStream.Network With {
                .Edges = edges,
                .Nodes = nodes.ToArray
            }
        End Function

        ''' <summary>
        ''' 从某一个分支点下来
        ''' </summary>
        ''' <param name="array"></param>
        ''' <param name="depth"></param>
        ''' <param name="nodes"></param>
        ''' <returns></returns>
        Private Function __buildNET(array As __edgePath(),
                                    parent As FileStream.Node,
                                    depth As Integer,
                                    ByRef nodes As List(Of FileStream.Node)) As NetworkEdge()

            Dim [next] As Integer = depth + 1  ' 下一层节点的深度

            If depth = array(Scan0).path.Length AndAlso
                array(Scan0).path.Last = "X"c Then
                Return array.ToArray(Function(x) New NetworkEdge With {
                                         .FromNode = parent.ID,
                                         .ToNode = x.node.Name,
                                         .InteractionType = "Leaf-X"})
            End If

            Dim Gp = (From x In array Let cur = x.path(depth) Select cur, x Group By cur Into Group).ToArray
            Dim edges As New List(Of NetworkEdge)

            For Each part In Gp
                Dim parts = part.Group.ToArray(Function(x) x.x)

                If parts.Length = 1 Then ' 叶节点
                    Dim leaf = parts.First
                    Call edges.Add(New NetworkEdge With {.FromNode = parent.ID, .ToNode = leaf.node.Name, .InteractionType = "Leaf"})
                Else                    ' 继续递归
                    Dim uid As String = $"[{part.cur}]" & parts.First.path.Take(depth).JoinBy(".")
                    Dim virtual As New FileStream.Node With {
                        .ID = uid,
                        .NodeType = "Virtual"
                    }
                    Call nodes.Add(virtual)
                    Call edges.Add(New NetworkEdge With {.FromNode = parent.ID, .ToNode = uid, .InteractionType = "Path"})
                    Call edges.Add(__buildNET(parts, virtual, [next], nodes))
                End If
            Next

            Return edges.ToArray
        End Function
    End Module
End Namespace
