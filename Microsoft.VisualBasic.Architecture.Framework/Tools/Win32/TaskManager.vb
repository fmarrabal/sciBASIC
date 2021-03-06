﻿#Region "Microsoft.VisualBasic::3bb4e0d076f3b410b94f9a9a2796f96b, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Tools\Win32\TaskManager.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Win32

    ''' <summary>
    ''' Windows的任务管理器的接口
    ''' </summary>
    Public Module TaskManager

        ''' <summary>
        ''' Using this property you can display the CPU usage (over all CPU usage like you would find on the task manager)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CPU_Usages As New PerformanceCounter("Processor", "% Processor Time", "_Total")

        ''' <summary>
        ''' 类似于任务管理器的函数：Memory, CPU, ProcessName, PID, CommandLine
        ''' </summary>
        ''' <returns>Memory, CPU</returns>
        ''' <remarks></remarks>
        Public Function ProcessUsageDetails() As List(Of TaskInfo)
            Dim counterList As New List(Of TaskInfo)

            Try
                Dim proc As Process() = Process.GetProcesses

                For Each P As Process In proc
                    Dim pCounter As New PerformanceCounter("Process", "% Processor Time", P.ProcessName)

                    counterList += New TaskInfo With {
                        .Memory = P.WorkingSet64,
                        .CPU = Math.Round(pCounter.NextValue, 2),
                        .ProcessName = P.ProcessName,
                        .PID = P.Id,
                        .CommandLine = P.StartInfo.FileName & " " & P.StartInfo.Arguments
                    }
                Next
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.PrintException
            End Try

            Return counterList
        End Function

        Public Structure TaskInfo

            <XmlAttribute> Public Property PID As Integer
            <XmlAttribute> Public Property CommandLine As String
            <XmlAttribute> Public Property ProcessName As String
            <XmlAttribute> Public Property CPU As Double
            <XmlAttribute> Public Property Memory As Long

            Default Public ReadOnly Property GetValue(name$) As Object
                Get
                    Select Case name
                        Case NameOf(PID)
                            Return PID

                        Case NameOf(CommandLine)
                            Return CommandLine

                        Case NameOf(ProcessName)
                            Return ProcessName

                        Case NameOf(CPU)
                            Return CPU

                        Case NameOf(Memory)
                            Return Memory

                        Case Else
                            Throw New Exception($"Unable found key '{name}' in: " & GetJson)
                    End Select
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Structure

        ''' <summary>
        ''' 获取CPU的使用率
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ProcessUsage() As Double
            Dim tasks As List(Of TaskInfo) = ProcessUsageDetails()
            Dim usage As Double = tasks _
                .Sum(Function(proc) CType(proc("CPU"), Double))
            Return usage
        End Function
    End Module
End Namespace
