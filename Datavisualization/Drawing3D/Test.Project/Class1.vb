﻿Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Drawing.Drawing3D

Public Class Class1 : Inherits Microsoft.VisualBasic.Drawing.Drawing3D.GDIDevice

    Protected m_vertices(8) As Point3D
    Protected m_faces(6, 4) As Integer
    Protected m_colors(6) As Color
    Protected m_brushes(6) As Brush
    Protected m_angle As Integer

    Protected Overrides Sub ___animationLoop()
        ' Update the variable after each frame.
        m_angle += 1
    End Sub

    Protected Overrides Sub __init()
        ' Create the cube vertices.
        m_vertices = New Point3D() {
                     New Point3D(-1, 1, -1),
                     New Point3D(1, 1, -1),
                     New Point3D(1, -1, -1),
                     New Point3D(-1, -1, -1),
                     New Point3D(-1, 1, 1),
                     New Point3D(1, 1, 1),
                     New Point3D(1, -1, 1),
                     New Point3D(-1, -1, 1)}

        ' Create an array representing the 6 faces of a cube. Each face is composed by indices to the vertex array
        ' above.
        m_faces = New Integer(,) {{0, 1, 2, 3}, {1, 5, 6, 2}, {5, 4, 7, 6}, {4, 0, 3, 7}, {0, 4, 5, 1}, {3, 2, 6, 7}}

        ' Define the colors of each face.
        m_colors = New Color() {Color.BlueViolet, Color.Cyan, Color.Green, Color.Yellow, Color.Violet, Color.LightSkyBlue}

        ' Create the brushes to draw each face. Brushes are used to draw filled polygons.
        For i = 0 To 5
            m_brushes(i) = New SolidBrush(m_colors(i))
        Next
    End Sub

    Protected Overrides Sub __updateGraphics(sender As Object, Gr As PaintEventArgs)
        Dim t(8) As Point3D
        Dim f(4) As Integer
        Dim v As Point3D
        Dim avgZ(6) As Double
        Dim order(6) As Integer
        Dim tmp As Double
        Dim iMax As Integer

        ' Clear the window
        Gr.Graphics.Clear(Color.LightBlue)

        ' Transform all the points and store them on the "t" array.
        For i = 0 To 7
            Dim b As Brush = New SolidBrush(Color.White)
            v = m_vertices(i)
            t(i) = v.RotateX(m_angle).RotateY(m_angle).RotateZ(Me.m_angle)
            t(i) = t(i).Project(Me.ClientSize.Width, Me.ClientSize.Height, 256, 4)
        Next

        ' Compute the average Z value of each face.
        For i = 0 To 5
            avgZ(i) = (t(m_faces(i, 0)).Z + t(m_faces(i, 1)).Z + t(m_faces(i, 2)).Z + t(m_faces(i, 3)).Z) / 4.0
            order(i) = i
        Next

        ' Next we sort the faces in descending order based on the Z value.
        ' The objective is to draw distant faces first. This is called
        ' the PAINTERS ALGORITHM. So, the visible faces will hide the invisible ones.
        ' The sorting algorithm used is the SELECTION SORT.
        For i = 0 To 4
            iMax = i
            For j = i + 1 To 5
                If avgZ(j) > avgZ(iMax) Then
                    iMax = j
                End If
            Next
            If iMax <> i Then
                tmp = avgZ(i)
                avgZ(i) = avgZ(iMax)
                avgZ(iMax) = tmp

                tmp = order(i)
                order(i) = order(iMax)
                order(iMax) = tmp
            End If
        Next

        ' Draw the faces using the PAINTERS ALGORITHM (distant faces first, closer faces last).
        For i = 0 To 5
            Dim points() As Point
            Dim index As Integer = order(i)
            points = New Point() {
                New Point(CInt(t(m_faces(index, 0)).X), CInt(t(m_faces(index, 0)).Y)),
                New Point(CInt(t(m_faces(index, 1)).X), CInt(t(m_faces(index, 1)).Y)),
                New Point(CInt(t(m_faces(index, 2)).X), CInt(t(m_faces(index, 2)).Y)),
                New Point(CInt(t(m_faces(index, 3)).X), CInt(t(m_faces(index, 3)).Y))
            }
            Gr.Graphics.FillPolygon(m_brushes(index), points)
        Next
    End Sub

End Class
