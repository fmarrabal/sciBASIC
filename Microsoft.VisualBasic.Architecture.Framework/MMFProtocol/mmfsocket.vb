﻿Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.MMFProtocol.MapStream
Imports Microsoft.VisualBasic.Text

Namespace MMFProtocol

    ''' <summary>
    ''' 客户端接受到的数据需要经过反序列化解码方能读取
    ''' </summary>
    ''' <param name="data"></param>
    ''' <remarks></remarks>
    Public Delegate Sub DataArrival(data As Byte())
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="message">UTF8 string</param>
    Public Delegate Sub ReadNewMessage(message As String)

    ''' <summary>
    ''' MMFProtocol socket object for the inter-process communication on the localhost, this can be using for the data exchange between two process.
    ''' </summary>
    ''' <remarks></remarks>
    <[Namespace]("MMFSocket", Description:="MMFProtocol socket object for the inter-process communication on the localhost, this can be using for the data exchange between two process.")>
    Public Class MMFSocket : Implements IDisposable

        Dim _MMFReader As MSIOReader, _MMFWriter As MSWriter
        Dim _UpdateFlag As Long
        Dim _CallBackHandler As DataArrival

        Public Property NewMessageCallBack As ReadNewMessage

        Public ReadOnly Property URI As String

        Sub New(uri As String)
            _MMFWriter = New MSWriter(uri)
            _MMFReader = New MSIOReader(uri)
            _URI = uri
        End Sub

        Public Const MMF_PROTOCOL As String = "mmf://"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <param name="dataArrivalCallBack">
        ''' Public Delegate Sub <see cref="__dataArrival"/>(byteData As <see cref="System.Byte"/>())
        ''' 会优先于事件<see cref="__dataArrival"></see>的发生</param>
        ''' <remarks></remarks>
        Sub New(uri As String, dataArrivalCallBack As DataArrival)
            Call Me.New(uri)
            _CallBackHandler = dataArrivalCallBack
            _MMFReader.DataArrivalCallBack = dataArrivalCallBack
        End Sub

        Public Sub SendMessage(byteData As Byte())
            Me._UpdateFlag = _MMFReader.Read.udtBadge + 1

            Dim bytData As Byte() = New MMFStream With {.byteData = byteData, .udtBadge = Me._UpdateFlag}.Serialize

            Call _MMFReader.Update(Me._UpdateFlag)
            Call _MMFWriter.WriteStream(bytData)
        End Sub

        Public Sub SendMessage(raw As Net.Protocol.RawStream)
            Call SendMessage(raw.Serialize)
        End Sub

        Public Sub SendMessage(str As String)
            Call Me.SendMessage(System.Text.Encoding.Unicode.GetBytes(str))
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call Me._MMFWriter.Free
                    Call Me._MMFReader.Free
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

        Public Overrides Function ToString() As String
            Return $"{MMF_PROTOCOL}{_URI}"
        End Function

        Public Function Ping() As Boolean
            Call SendMessage(str:=_PING_MESSAGE)
            Call System.Threading.Thread.Sleep(100)

            If PingResult = True Then
                PingResult = False
                Return True
            Else
                Return False
            End If
        End Function

        Private Const _PING_MESSAGE As String = "PINT_REQUEST::{BAB0B3FA-C3F3-42F2-A577-9AF57EBDB9C7}"
        Private Const _PING_RETURNS As String = "PING_RETURNS::{BAB0B3FA-C3F3-42F2-A577-9AF57EBDB9C7}"

        Dim PingResult As Boolean = False

        Private Sub __dataArrival(data() As Byte)
            Dim s_MSG As String = System.Text.Encoding.Unicode.GetString(data)

            If String.Equals(s_MSG, _PING_MESSAGE) Then
                Call SendMessage(_PING_RETURNS)
            ElseIf String.Equals(s_MSG, _PING_RETURNS) Then
                PingResult = True
            Else
                If Not _CallBackHandler Is Nothing Then
                    Call _CallBackHandler(data)
                End If

                If Not NewMessageCallBack Is Nothing Then
                    Call NewMessageCallBack()(s_MSG)
                End If
            End If
        End Sub

#Region "ShellScript API"

        <ExportAPI("MMFProtocol.New")>
        Public Shared Function CreateObject(hostName As String, Optional handler As Action(Of Generic.IEnumerable(Of Byte)) = Nothing) As MMFProtocol.MMFSocket
            Dim CallBackHandle As DataArrival = New DataArrival(Sub(data As Byte()) handler(data))
            Return New MMFSocket(hostName, CallBackHandle)
        End Function

        <ExportAPI("SendMessage")>
        Public Shared Function SendMessage(socket As MMFSocket, data As Generic.IEnumerable(Of Byte)) As Boolean
            Try
                Call socket.SendMessage(data.ToArray)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        <ExportAPI("getMessage")>
        Public Shared Function getMessage(data As Generic.IEnumerable(Of Byte), Optional encoding As String = "") As String
            Return TextFileEncodingDetector.ToString(data, encoding)
        End Function

        <ExportAPI("Print.Message")>
        Public Shared Function WriteMessage(data As Generic.IEnumerable(Of Byte)) As String
            Dim Message As String = getMessage(data)
            Call Console.WriteLine(Message)
            Return Message
        End Function
#End Region
    End Class
End Namespace