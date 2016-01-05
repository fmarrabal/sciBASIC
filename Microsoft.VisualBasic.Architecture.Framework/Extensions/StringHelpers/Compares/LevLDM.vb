﻿Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Public Class Vector
    <Xml.Serialization.XmlAttribute> Public Property Value As Double()

    Default Public Property x(index As Integer) As Double
        Get
            Return Value(index)
        End Get
        Set(value As Double)
            Me.Value(index) = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return String.Join(", ", Value.ToArray(Function(n) CStr(n)))
    End Function

    Public Overloads Shared Widening Operator CType(vec As Double()) As Vector
        Return New Vector With {.Value = vec}
    End Operator
End Class

Public Class DistResult

    Public Property Reference As String
    Public Property Hypotheses As String
    Public Property DistTable As Vector()
    ''' <summary>
    ''' How doest the <see cref="Hypotheses"/> evolve from <see cref="Reference"/>.(这个结果描述了subject是如何变化成为Query的)
    ''' </summary>
    ''' <returns></returns>
    Public Property DistEdits As String
    Public Property CSS As Point()
    Public Property Matches As String

    Public Overrides Function ToString() As String
        Return $"{Reference} => {Hypotheses} // {DistEdits}"
    End Function

    Public ReadOnly Property Distance As Double
        Get
            If DistTable.IsNullOrEmpty Then
                Return 0
            End If

            Dim XMax = CSS.ToArray(Function(c) c.X).Max
            Dim YMax = CSS.ToArray(Function(c) c.Y).Max
            Return DistTable(XMax)(YMax)
        End Get
    End Property

    ''' <summary>
    ''' 可以简单地使用这个数值来表述所比较的两个对象之间的相似度
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Score As Double
        Get
            If String.IsNullOrEmpty(DistEdits) Then
                Return 0
            End If

            Dim view As String = DistEdits.ToLower
            Dim m As Integer = view.Count("m"c)
            Dim i As Integer = view.Count("i"c)
            Dim d As Integer = view.Count("d"c)
            Dim s As Integer = view.Count("s"c)
            Dim len As Integer = view.Length

            Return (m - (i * 0.5 + d * 0.3 + s * 0.2)) / len
        End Get
    End Property

    Public ReadOnly Property Similarity As Double
        Get
            Dim d As Double = DistTable.Last.Value.Last
            Dim maxLength As Integer = Math.Max(DistTable.Length, DistTable.First.Value.Length)
            Dim value As Double = 1.0 - d / maxLength
            Return value
        End Get
    End Property

    ''' <summary>
    ''' 比对上的对象的数目
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NumMatches As Integer
        Get
            If String.IsNullOrEmpty(DistEdits) Then
                Return 0
            End If

            Dim view As String = DistEdits.ToLower
            Dim m As Integer = view.Count("m"c)
            Return m
        End Get
    End Property

    Public Function CopyTo(Of T As DistResult)(ByRef obj As T) As T
        obj.CSS = CSS
        obj.DistEdits = DistEdits
        obj.DistTable = DistTable
        obj.Hypotheses = Hypotheses
        obj.Matches = Matches
        obj.Reference = Reference
        Return obj
    End Function

    Public Function TrimMatrix(l As Integer) As Vector()
        Me.DistTable = Me.DistTable.ToArray(
            Function(row) New Vector With {
                .Value = row.Value.ToArray(Function(n) Math.Round(n, l))
            })
        Return Me.DistTable
    End Function

    ''' <summary>
    ''' Output HTML result for visualization
    ''' </summary>
    ''' <returns></returns>
    Public Function Visualize() As String
        Try
            Return __visualizeHTML()
        Catch ex As Exception
            Call App.LogException(ex)
            Return $"<!DOCTYPE HTML>
<html>
<head>
<title>{NameOf(HTTP_RFC.RFC_INTERNAL_SERVER_ERROR)}</title>
</head>
<body>
<pre>{ex.ToString}</pre>
</body>
</html>"
        End Try
    End Function

    Private Function __visualizeHTML() As String
        Dim html As StringBuilder = New StringBuilder(1024)
        Call html.AppendLine($"<!DOCTYPE HTML>
<html>
<head>
<title>{ToString()}</title>
</head>
")
        Call html.AppendLine("<body style=""font-family:Ubuntu;"">")
        Call html.AppendLine("<h3>Summary</h3>")
        Call html.AppendLine($"<table>
<tr>
<td>Reference: </td><td> ({__getReference.Length}) <strong> {Reference}</strong></td>
</tr>
<tr>
<td>Hypotheses: </td><td> ({__getSubject.Length}) <strong> {Hypotheses}</strong></td>
</tr>
<tr>
<td>Levenshtein Edit: </td><td> ({Len(DistEdits) - DistEdits.Count("m"c)}/{Len(DistEdits)}) <strong> {DistEdits}</strong></td>
</tr>
<tr>
<td>Matches: </td><td> ({DistEdits.Count("m"c)}/{Len(DistEdits)}) <strong> {Matches}</strong></td>
</tr>
{__innerInsert()}
<tr>
<td>Distance: </td><td> <strong> {Distance}</strong></td>
<tr>
<tr>
<td>Score: </td><td> <strong> {Score}</strong></td>
</tr>
</table>")
        Call html.AppendLine("<p>
<table>
<tr><td>d</td><td> -> Delete</td></tr>
<tr><td>i</td><td> -> Insert</td></tr>
<tr><td>m</td><td> -> Match</td></tr>
<tr><td>s</td><td> -> Substitute</td></tr>
</table>
</p>")
        Call html.AppendLine("<h3>Levenshtein Edit Distance Table</h3>")
        Call html.AppendLine(__innerMatrix)
        Call html.AppendLine("</body></html>")

        Return html.ToString
    End Function

    Protected Overridable Function __innerInsert() As String
        Return ""
    End Function

    Private Function __innerMatrix() As String
        Dim MAT As StringBuilder = New StringBuilder("<table>")
        Dim dict As Dictionary(Of Integer, Integer())

        If DistTable Is Nothing Then
            Return ""
        End If

        If CSS.IsNullOrEmpty Then
            dict = New Dictionary(Of Integer, Integer())
        Else
            dict = (From cell As Point
                    In CSS
                    Select cell
                    Group cell By cell.X Into Group) _
                        .ToDictionary(Function(row) row.X,
                                      Function(row) row.Group.ToArray.ToArray(Function(cell) cell.Y))
        End If

        Dim Reference As String = __getReference()
        Dim Hypotheses As String = __getSubject()

        Call MAT.AppendLine($"<tr><td><tr><td></td>
{Hypotheses.ToArray(Function(ch) $"<td><strong>{ch.ToString}</strong></td>").JoinBy("")}</tr>")

        For i As Integer = 0 To Len(Reference) - 1
            Dim row As New StringBuilder()

            For j As Integer = 0 To Len(Hypotheses) - 1
                If dict.ContainsKey(i) AndAlso Array.IndexOf(dict(i), j) > -1 Then
                    Call row.Append($"<td style=""background-color:green;color:white""><strong>{DistTable(i)(j)}</strong></td>")
                Else
                    Call row.Append($"<td>{DistTable(i)(j)}</td>")
                End If
            Next
            Call MAT.AppendLine($"<tr><td><strong>{Reference(i).ToString}</strong></td>
{row.ToString}
<td style=""background-color:blue;color:white""><strong>{Me.DistEdits(i).ToString}</strong></td></tr>")
        Next

        Call MAT.AppendLine("</table>")

        Return MAT.ToString
    End Function

    Protected Overridable Function __getReference() As String
        Return Reference
    End Function

    Protected Overridable Function __getSubject() As String
        Return Hypotheses
    End Function
End Class
