﻿Imports System.Text

Namespace ConsoleDevice

    Public Class InteractiveDevice : Inherits ConsoleDevice.I_SystemConsoleObject
        Implements System.IDisposable
        Implements Microsoft.VisualBasic.ComponentModel.ITextFile.I_FileSaveHandle

        Dim _InternalStreamBuffer As StringBuilder = New StringBuilder(2048)
        Dim _InternalCommandsHistory As ConsoleDevice.HistoryStacks
        Dim Blanks As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s"></param>
        ''' <remarks></remarks>
        Public Event NewOutputMessage(s As String)

        Public Property PromptText As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="HistoryFile">历史数据文件的存放位置，假若为空，则使用默认文件路径</param>
        ''' <remarks></remarks>
        Sub New(Optional HistoryFile As String = "")
            If String.IsNullOrEmpty(HistoryFile) Then
                HistoryFile = $"{Application.ExecutablePath}.commandline_histories.dat"
            End If

            Try
                _InternalCommandsHistory = HistoryFile.LoadXml(Of HistoryStacks)()
            Catch ex As Exception
                _InternalCommandsHistory = New HistoryStacks
                Call _InternalCommandsHistory.Save(HistoryFile)
            End Try

            If _InternalCommandsHistory Is Nothing Then
                _InternalCommandsHistory = New HistoryStacks
            End If

            _InternalCommandsHistory.FilePath = HistoryFile
            Call _InternalCommandsHistory.StartInitialize()

            PromptText = "#"
            Blanks = New String(" "c, Console.BufferWidth)
        End Sub

        Dim _InternalCacheReadLine As String
        Dim _EmptyHistory As Boolean, _historyControl As Boolean = True

        Public Overrides Function ReadKey() As ConsoleKeyInfo
            Dim n = Console.ReadKey

            _historyControl = True

            Select Case n.Key
                Case ConsoleKey.UpArrow
                    _InternalCacheReadLine = _InternalCommandsHistory.MovePrevious
                Case ConsoleKey.DownArrow
                    _InternalCacheReadLine = _InternalCommandsHistory.MoveNext
                Case ConsoleKey.Home
                    _InternalCacheReadLine = _InternalCommandsHistory.MoveFirst
                Case ConsoleKey.End
                    _InternalCacheReadLine = _InternalCommandsHistory.MoveLast
                Case Else
                    _historyControl = False
            End Select

            If _historyControl Then
                Call Console.SetCursorPosition(Console.CursorLeft - 1, CursorTop)       '回移一格，因为控制符也会被输出的
            End If

            If Not String.IsNullOrEmpty(_InternalCacheReadLine) Then
                Call InternalClearLine(Console.CursorTop)
                Call Console.Write(_InternalCacheReadLine)
                _EmptyHistory = False
            ElseIf Not String.IsNullOrEmpty(HistoryCallerStack) Then
                _InternalCacheReadLine = HistoryCallerStack
                HistoryCallerStack = ""
                _EmptyHistory = False
            Else
                _InternalCacheReadLine = ""
                _EmptyHistory = True   '空的历史
            End If

            Return n
        End Function

        ''' <summary>
        ''' ReadLine函数的递归返回值
        ''' </summary>
        ''' <remarks></remarks>
        Dim HistoryCallerStack As String

        Public Overrides Function ReadLine() As String
            Dim strCommand As String = "", n = Me.ReadKey()

            If _historyControl Then '用户浏览了历史记录
                HistoryCallerStack = _InternalCacheReadLine
                Return Me.ReadLine
            Else
                If n.Key = ConsoleKey.Enter Then '用户输入了数据
                    strCommand = _InternalCacheReadLine
                Else '用户还没有完成输入
EXIT_INPUT:         strCommand = HistoryCallerStack & n.KeyChar & MyBase.ReadLine
                End If
            End If

            HistoryCallerStack = ""
            _InternalCacheReadLine = ""

            '            If Not String.IsNullOrEmpty(_InternalCacheReadLine) Then
            '                strCommand = n.KeyChar & _InternalCacheReadLine
            '                _InternalCacheReadLine = ""
            '                If Not n.Key = ConsoleKey.Enter Then
            '                    If _historyControl Then
            '                        '用户调出了历史，但是没有按下回车，可能还会继续浏览历史
            '                        HistoryCallerStack = strCommand
            '                        Return Me.ReadLine()   '上一个按键是方向键
            '                    Else
            '                        GoTo EXIT_INPUT       '不是方向键则输出
            '                    End If
            '                Else
            '                    If _EmptyHistory Then
            '                        Call Me.PrintPrompted(Lf:=True)
            '                        Return Me.ReadLine()
            '                    Else
            '                        Call Console.WriteLine()
            '                    End If
            '                End If
            '            Else
            '                If _historyControl Then '历史记录是空的，但是任然算是浏览了历史记录
            '                    Return Me.ReadLine
            '                End If

            '                If n.Key = ConsoleKey.Enter Then
            '                    Return Me.ReadLine           '没有输入，则换行并要求重新输入
            '                Else
            'EXIT_INPUT:         strCommand = strCommand & MyBase.ReadLine
            '                End If
            '            End If

            Call _InternalCommandsHistory.PushStack(strCommand)

            Return strCommand
        End Function

        Public Sub PrintPrompted(Optional Lf As Boolean = False)
            If Lf Then Call Console.WriteLine()
            Call Console.Write(PromptText & "  ")
        End Sub

        Private Sub InternalClearLine(top As Integer)
            Dim current As Integer = Console.CursorTop

            Call Console.SetCursorPosition(Len(PromptText) + 2, top)
            Call Console.Write(Blanks)
            Call Console.SetCursorPosition(Len(PromptText) + 2, current)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Call Me.Save()
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(      disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(      disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        ''' <summary>
        ''' 保存历史数据
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ComponentModel.ITextFile.I_FileSaveHandle.Save
            Return _InternalCommandsHistory.Save(Path, encoding)
        End Function
    End Class
End Namespace