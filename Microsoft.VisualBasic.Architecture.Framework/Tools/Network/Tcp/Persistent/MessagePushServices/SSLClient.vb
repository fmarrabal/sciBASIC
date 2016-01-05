﻿
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports Microsoft.VisualBasic.Net.Abstract
Imports Microsoft.VisualBasic.Net.Protocol
Imports Microsoft.VisualBasic.Net.Protocol.Reflection

Namespace Net.Persistent.Application

    Public Class SSLClient

        Public ReadOnly Property PrivateKey As Net.SSL.Certificate
        Public ReadOnly Property PushUser As Net.Persistent.Application.USER

        ReadOnly _DataRequestHandle As PushMessage

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="services"></param>
        ''' <param name="ID"></param>
        ''' <param name="DataRequestHandle">Public Delegate Function PushMessage(USER_ID As <see cref="Long"/>, Message As <see cref="RequestStream"/>) As <see cref="RequestStream"/></param>
        ''' <param name="ExceptionHandler"></param>
        Sub New(services As System.Net.IPEndPoint, ID As Long, DataRequestHandle As PushMessage, Optional ExceptionHandler As ExceptionHandler = Nothing)
            _DataRequestHandle = DataRequestHandle
            PushUser = New USER(services, ID, AddressOf __sslRedirect, ExceptionHandler)
        End Sub

        Private Function __sslRedirect(USER_ID As Long, request As RequestStream) As RequestStream
            request = PrivateKey.Decrypt(request)  ' 解密之后在讲数据传递到实际的业务逻辑之上
            request = _DataRequestHandle(USER_ID, request)
            Return request
        End Function

        Public Sub Handshaking(PublicToken As Net.SSL.Certificate)
            Dim Services = New System.Net.IPEndPoint(System.Net.IPAddress.Parse(PushUser.remoteHost), PushUser.remotePort)
            _PrivateKey = Net.SSL.Certificate.CopyFrom(PublicToken, PushUser.USER_ID)
            _PrivateKey = Net.SSL.SSLProtocols.Handshaking(PrivateKey, Services)
            Call PushUser.BeginConnect(PrivateKey, _disconnectHandler)
        End Sub

        ''' <summary>
        ''' 使用已经拥有的用户证书登录服务器，这一步省略了握手步骤
        ''' </summary>
        ''' <param name="UserToken"></param>
        Public Sub Logon(UserToken As Net.SSL.Certificate)
            _PrivateKey = UserToken
            _PushUser.BeginConnect(UserToken, _disconnectHandler)
        End Sub

        Dim _disconnectHandler As MethodInvoker

        Public Sub SetDisconnectHandle([handle] As MethodInvoker)
            _disconnectHandler = handle
            _PushUser.SetDisconnectHandle(handle)
        End Sub

        ''' <summary>
        ''' 消息在这个函数之中自动被加密处理
        ''' </summary>
        ''' <param name="USER_ID"></param>
        ''' <param name="request"></param>
        ''' <returns></returns>
        Public Function SendMessage(USER_ID As Long, request As RequestStream) As Boolean
            Return PushUser.SendMessage(USER_ID, request, PrivateKey)
        End Function
    End Class
End Namespace