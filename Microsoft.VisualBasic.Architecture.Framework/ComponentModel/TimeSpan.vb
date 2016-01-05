﻿
<Xml.Serialization.XmlTypeAttribute("Interval", Namespace:="Microsoft.VisualBasic/Numerical_DataStruct")>
Public Structure TimeInterval

    <Xml.Serialization.XmlAttribute("dd")> Public Property Days As Integer
    <Xml.Serialization.XmlAttribute("min")> Public Property Minutes As Integer
    <Xml.Serialization.XmlAttribute("hr")> Public Property Hours As Integer
    <Xml.Serialization.XmlAttribute("ss")> Public Property Seconds As Integer
    <Xml.Serialization.XmlAttribute("ms")> Public Property Miliseconds As Integer

    Sub New(TimeSpan As TimeSpan)
        Days = TimeSpan.Days
        Minutes = TimeSpan.Minutes
        Hours = TimeSpan.Hours
        Seconds = TimeSpan.Seconds
        Miliseconds = TimeSpan.Milliseconds
    End Sub

    Public ReadOnly Property TimeSpan As TimeSpan
        Get
            Return New TimeSpan(Days, Hours, Minutes, Seconds, Miliseconds)
        End Get
    End Property

    ''' <summary>
    ''' (dd hh:mm:ss) 输出可以被MySQL数据库所识别的字符串格式
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Return String.Format("{0} {1}:{2}:{3}", Days, Hours, Minutes, Seconds)
    End Function

    Public ReadOnly Property Ticks As Long
        Get
            Return TimeSpan.Ticks
        End Get
    End Property

    Public Shared Widening Operator CType(TimeSpan As TimeSpan) As TimeInterval
        Return New TimeInterval(TimeSpan)
    End Operator
End Structure
