﻿'Copyright (C) 2016 Guillermo Hazebrouck

Imports MathTools.Algebra.EuclideanSpace
Imports AeroTools.VisualModel.Interface
Imports SharpGL
Imports System.Xml
Imports MathTools

Namespace VisualModel.Models.Basics

    ''' <summary>
    ''' Basic definition of a base surface able to be operated and selected.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class BaseSurface

        Implements IOperational
        Implements ISelectable

        ''' <summary>
        ''' Surface identifier.
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As Guid

        ''' <summary>
        ''' Surface name.
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' Mesh.
        ''' </summary>
        ''' <returns></returns>
        Public Property Mesh As Mesh

        ''' <summary>
        ''' Surface visual properties.
        ''' </summary>
        ''' <returns></returns>
        Public Property VisualProps As VisualizationProperties

        ''' <summary>
        ''' Indicates if the surface participates in the calculation model.
        ''' </summary>
        ''' <returns></returns>
        Public Property IncludeInCalculation As Boolean = False

        ''' <summary>
        ''' Indicate if the GUI has to block the content of this surface.
        ''' </summary>
        ''' <returns></returns>
        Public Property LockContent As Boolean = True

        ''' <summary>
        ''' Position of the surface in the global coordinates system.
        ''' </summary>
        ''' <returns></returns>
        Public Property Position As New EVector3

        ''' <summary>
        ''' Center of rotation of the surface in the local coordinates system.
        ''' </summary>
        ''' <returns></returns>
        Public Property CenterOfRotation As New EVector3

        ''' <summary>
        ''' Orientation of the surface.
        ''' </summary>
        ''' <returns></returns>
        Public Property Orientation As New OrientationCoordinates

        ''' <summary>
        ''' Scale of this surface.
        ''' </summary>
        ''' <returns></returns>
        Public Property SizeScale As Double = 1.0

#Region " Operations "

        Public Overridable Sub Translate(ByVal Vector As EVector3) Implements IOperational.Translate

            Mesh.Translate(Vector)

        End Sub

        Public Overridable Sub Orientate(ByVal Point As EVector3, ByVal Ori As OrientationCoordinates) Implements IOperational.Orientate

            Mesh.Rotate(Point, Ori)

        End Sub

        Public Overridable Sub Scale(ByVal Scale As Double) Implements IOperational.Scale

            Mesh.Scale(Scale)

        End Sub

        Public Overridable Sub Align(ByVal P1 As EVector3, ByVal P2 As EVector3, ByVal P3 As EVector3, ByVal P4 As EVector3) Implements IOperational.Align

            Mesh.Align()

        End Sub

#End Region

        ''' <summary>
        ''' Indicates if the surface is currently selected.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Selected As Boolean = False Implements ISelectable.Selected

        ''' <summary>
        ''' Unselects all nodal points.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UnselectAll() Implements ISelectable.UnselectAll

            For Each node In Mesh.NodalPoints
                node.Active = False
            Next

        End Sub

#Region " Meshing "

        ''' <summary>
        ''' Refresh the 3D model on a OpenGL control.
        ''' </summary>
        ''' <param name="gl"></param>
        ''' <param name="SelectionMode"></param>
        ''' <param name="ElementIndex"></param>
        ''' <remarks></remarks>
        Public MustOverride Sub Refresh3DModel(ByRef gl As OpenGL, Optional ByVal SelectionMode As SelectionModes = SelectionModes.smNoSelection, Optional ByVal ElementIndex As Integer = 0)

        Public MustOverride Sub GenerateMesh()

#End Region

#Region " IO "

        Public MustOverride Sub WriteToXML(ByRef writes As XmlWriter)

        Public MustOverride Sub ReadFromXML(ByRef reader As XmlReader)

#End Region

    End Class

End Namespace