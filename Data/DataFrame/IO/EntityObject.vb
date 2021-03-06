﻿#Region "Microsoft.VisualBasic::b14e8a3148a3b68af9768a7da00701eb, ..\sciBASIC#\Data\DataFrame\IO\EntityObject.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace IO

    ''' <summary>
    ''' The object entity, <see cref="DynamicPropertyBase(Of String)"/>, <see cref="String"/>
    ''' </summary>
    Public Class EntityObject : Inherits DynamicPropertyBase(Of String)
        Implements INamedValue

        ''' <summary>
        ''' This object identifier
        ''' </summary>
        ''' <returns></returns>
        <Column("ID")>
        Public Property ID As String Implements INamedValue.Key

        ''' <summary>
        ''' Copy prop[erty value
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Function Copy() As EntityObject
            Return New EntityObject With {
                .ID = ID,
                .Properties = New Dictionary(Of String, String)(Properties)
            }
        End Function

        Public Shared Function LoadDataSet(path As String, Optional uidMap As String = Nothing) As IEnumerable(Of EntityObject)
            If uidMap Is Nothing Then
                Dim first As New RowObject(path.ReadFirstLine)
                uidMap = first.First
            End If
            Dim map As New Dictionary(Of String, String) From {
                {uidMap, NameOf(EntityObject.ID)}
            }
            Return path.LoadCsv(Of EntityObject)(explicit:=False, maps:=map)
        End Function
    End Class

    ''' <summary>
    ''' The numeric dataset, <see cref="DynamicPropertyBase(Of Double)"/>, <see cref="Double"/>.
    ''' (数值类型的数据集合，每一个数据实体对象都有自己的编号以及数据属性)
    ''' </summary>
    Public Class DataSet : Inherits DynamicPropertyBase(Of Double)
        Implements INamedValue

        Public Property ID As String Implements INamedValue.Key

        ''' <summary>
        ''' Copy prop[erty value
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Function Copy() As DataSet
            Return New DataSet With {
                .ID = ID,
                .Properties = New Dictionary(Of String, Double)(Properties)
            }
        End Function

        ''' <summary>
        ''' <paramref name="uidMap"/>一般情况下会自动进行判断，不需要具体的设置
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="uidMap">
        ''' 默认是使用csv文件的第一行第一个单元格中的内容作为标识符，但是有时候可能标识符不是在第一列的，则这个时候就需要对这个参数进行赋值了
        ''' </param>
        ''' <returns></returns>
        Public Shared Function LoadDataSet(path$, Optional uidMap$ = Nothing) As IEnumerable(Of DataSet)
            If uidMap Is Nothing Then
                Dim first As New RowObject(path.ReadFirstLine)
                uidMap = first.First
            End If
            Dim map As New Dictionary(Of String, String) From {
                {uidMap, NameOf(DataSet.ID)}
            }
            Return path.LoadCsv(Of DataSet)(explicit:=False, maps:=map)
        End Function
    End Class
End Namespace
