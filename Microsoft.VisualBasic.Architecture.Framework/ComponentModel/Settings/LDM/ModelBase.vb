﻿#Region "Microsoft.VisualBasic::9161dffe0553bacd8f508b50bbf6d0c4, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\ComponentModel\Settings\LDM\ModelBase.vb"

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

Namespace ComponentModel.Settings

    ''' <summary>
    ''' 具备有保存数据功能的可配置数据文件的基本定义
    ''' </summary>
    Public Interface IProfile

        Function Save(Optional FilePath As String = "", Optional Encoding As System.Text.Encoding = Nothing) As Boolean

        ''' <summary>
        ''' 本属性不能够被设置为只读属性是因为 Settings.Settings(Of IProfile).LoadFile 函数的需要
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property FilePath As String
    End Interface
End Namespace
