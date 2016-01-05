﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("LINQ")>
Public Module LINQ

    ''' <summary>
    ''' Copy <paramref name="source"/> <paramref name="n"/> times to construct a new vector.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="n"></param>
    ''' <returns>An array consist of source with n elements.</returns>
    <Extension> Public Function CopyVector(Of T)(source As T, n As Integer) As T()
        Return n.ToArray(Function(x) source)
    End Function

    <Extension> Public Function Read(Of T)(array As T(), ByRef i As Integer, ByRef out As T) As T
        out = array(i)
        i += 1
        Return out
    End Function

    ''' <summary>
    ''' 产生指定数目的一个递增序列(所生成序列的数值就是生成的数组的元素的个数)
    ''' </summary>
    ''' <param name="n">大于或者等于0的一个数，当小于0的时候会出错</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Sequence")>
    <Extension> Public Function Sequence(n As Integer) As Integer()

        If n < 0 Then
            Dim ex As String = $"n:={n} is not a valid index generator value for sequence!"
            Throw New Exception(ex)
        End If

        Dim List(n - 1) As Integer
        For i As Integer = 0 To n - 1
            List(i) = i
        Next
        Return List
    End Function

    ''' <summary>
    ''' 产生指定数目的一个递增序列(所生成序列的数值就是生成的数组的元素的个数)
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Sequence")>
    <Extension> Public Function Sequence(n As Long) As Long()
        Dim List As Long() = New Long(n - 1) {}
        For i As Integer = 0 To n - 1
            List(i) = i
        Next
        Return List
    End Function

    ''' <summary>
    ''' 产生指定数目的一个递增序列(所生成序列的数值就是生成的数组的元素的个数)
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Sequence")>
    <Extension> Public Function Sequence(n As UInteger) As Integer()
        Dim List(n - 1) As Integer
        For i As Integer = 0 To n - 1
            List(i) = i
        Next
        Return List
    End Function

    ''' <summary>
    ''' (所生成序列的数值就是生成的数组的元素的个数)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="len"></param>
    ''' <param name="elementAt"></param>
    ''' <returns></returns>
    <Extension> Public Function ToArray(Of T)(len As Integer, elementAt As Func(Of Integer, T)) As T()
        Return len.Sequence.ToArray(elementAt)
    End Function

    ''' <summary>
    ''' Creates an array from a System.Collections.Generic.IEnumerable`1.(默认非并行化的，这个函数是安全的，假若参数为空值则会返回一个空的数组)
    ''' </summary>
    ''' <typeparam name="T">The type of the elements of source.</typeparam>
    ''' <typeparam name="TOut"></typeparam>
    ''' <param name="source">An System.Collections.Generic.IEnumerable`1 to create an array from.</param>
    ''' <returns>An array that contains the elements from the input sequence.</returns>
    <Extension> Public Function ToArray(Of T, TOut)(source As Generic.IEnumerable(Of T),
                                                    [CType] As Func(Of T, TOut),
                                                    Optional Parallel As Boolean = False) As TOut()
        If source.IsNullOrEmpty Then
            Return New TOut() {}
        End If

        Dim LQuery As TOut()

        If Parallel Then
            LQuery = (From obj As T In source.AsParallel Select [CType](obj)).ToArray
        Else
            LQuery = (From obj As T In source Select [CType](obj)).ToArray
        End If

        Return LQuery
    End Function

    ''' <summary>
    ''' Creates an array from a System.Collections.Generic.IEnumerable`1.(默认非并行化的，这个函数是安全的，假若参数为空值则会返回一个空的数组)
    ''' </summary>
    ''' <typeparam name="T">The type of the elements of source.</typeparam>
    ''' <typeparam name="TOut"></typeparam>
    ''' <param name="source">An System.Collections.Generic.IEnumerable`1 to create an array from.</param>
    ''' <returns>An array that contains the elements from the input sequence.</returns>
    <Extension> Public Function ToArray(Of T, TOut)(source As Generic.IEnumerable(Of T),
                                                    [CType] As Func(Of T, TOut),
                                                    [where] As Func(Of T, Boolean),
                                                    Optional Parallel As Boolean = False) As TOut()
        If source.IsNullOrEmpty Then
            Return New TOut() {}
        End If

        Dim LQuery As TOut()

        If Parallel Then
            LQuery = (From obj As T In source.AsParallel
                      Where where(obj)
                      Select [CType](obj)).ToArray
        Else
            LQuery = (From obj As T In source
                      Where where(obj)
                      Select [CType](obj)).ToArray
        End If

        Return LQuery
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="TOut"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="[CType]">第二个参数是index</param>
    ''' <param name="Parallel"></param>
    ''' <returns></returns>
    <Extension> Public Function ToArray(Of T, TOut)(source As Generic.IEnumerable(Of T),
                                                    [CType] As Func(Of T, Integer, TOut),
                                                    Optional Parallel As Boolean = False) As TOut()
        If source.IsNullOrEmpty Then
            Return New TOut() {}
        End If

        Dim LQuery As TOut()
        Dim index As Integer() = source.Sequence

        If Parallel Then
            LQuery = (From i As Integer In index.AsParallel Let obj As T = source(i) Select [CType](obj, i)).ToArray
        Else
            LQuery = (From i As Integer In index Let obj As T = source(i) Select [CType](obj, i)).ToArray
        End If

        Return LQuery
    End Function

    <Extension> Public Function ToArray(Of T)(len As Long, elementAt As Func(Of Long, T)) As T()
        Return len.Sequence.ToArray(elementAt)
    End Function

    <Extension> Public Function FirstOrDefault(Of TSource)(source As IEnumerable(Of TSource), [default] As TSource) As TSource
        Dim value As TSource = source.FirstOrDefault
        If value Is Nothing Then
            Return [default]
        Else
            Return value
        End If
    End Function

    Public Function ToArray(source As IEnumerable) As Object()
        Dim LQuery As Object() = (From x As Object In source Select x).ToArray
        Return LQuery
    End Function
End Module
