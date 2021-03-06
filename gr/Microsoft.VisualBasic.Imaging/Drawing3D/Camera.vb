﻿#Region "Microsoft.VisualBasic::0f74cfc0985efcf3898983483fc6adae, ..\sciBASIC#\gr\Microsoft.VisualBasic.Imaging\Drawing3D\Camera.vb"

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

Imports System.Drawing
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Language

Namespace Drawing3D

    Public Class Camera

        Public ViewDistance!, angleX!, angleY!, angleZ!
        Public fov! = 256.0!
        Public screen As Size
        ''' <summary>
        ''' Using for the project result 
        ''' </summary>
        Public offset As Point

#Region "Rotation"

        Public Function Rotate(pt As Point3D) As Point3D
            Return pt.RotateX(angleX).RotateY(angleY).RotateZ(angleZ)
        End Function

        Public Function RotateX(pt As Point3D) As Point3D
            Return pt.RotateX(angleX)
        End Function

        Public Function RotateY(pt As Point3D) As Point3D
            Return pt.RotateY(angleY)
        End Function

        Public Function RotateZ(pt As Point3D) As Point3D
            Return pt.RotateZ(angleZ)
        End Function

        Public Iterator Function Rotate(pts As IEnumerable(Of Point3D)) As IEnumerable(Of Point3D)
            For Each pt As Point3D In pts
                Yield pt _
                    .RotateX(angleX) _
                    .RotateY(angleY) _
                    .RotateZ(angleZ)
            Next
        End Function

        Public Iterator Function RotateX(pts As IEnumerable(Of Point3D)) As IEnumerable(Of Point3D)
            For Each pt As Point3D In pts
                Yield pt.RotateX(angleX)
            Next
        End Function

        Public Iterator Function RotateY(pts As IEnumerable(Of Point3D)) As IEnumerable(Of Point3D)
            For Each pt As Point3D In pts
                Yield pt.RotateY(angleY)
            Next
        End Function

        Public Iterator Function RotateZ(pts As IEnumerable(Of Point3D)) As IEnumerable(Of Point3D)
            For Each pt As Point3D In pts
                Yield pt.RotateZ(angleZ)
            Next
        End Function
#End Region

#Region "3D -> 2D Project"

        Public Function Project(pt As Point3D) As Point3D
            Return pt.Project(screen.Width, screen.Height, fov, ViewDistance, offset)
        End Function

        Public Iterator Function Project(pts As IEnumerable(Of Point3D)) As IEnumerable(Of Point3D)
            For Each pt As Point3D In pts
                Yield pt.Project(screen.Width, screen.Height, fov, ViewDistance, offset)
            Next
        End Function

#End Region

        Public Sub Draw(ByRef canvas As Graphics, surface As IEnumerable(Of Surface), Optional drawPath As Boolean = False)
            Dim faces As New List(Of Surface)

            With Me
                For Each f As Surface In surface
                    faces += New Surface With {
                        .brush = f.brush,
                        .vertices = Rotate(f.vertices).ToArray
                    }
                Next
            End With

            Call canvas.SurfacePainter(Me, faces, drawPath)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
