﻿'Copyright (C) 2016 Guillermo Hazebrouck

Imports AeroTools.VisualModel.Models.Basics
Imports MathTools.Algebra.EuclideanSpace

Public Class FormAskVelocity

    Private Result As String

    Private Sub PreguntarVelocidad_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PuntoDeControl.Minimum = 1
        PuntoDeControl.Maximum = MainForm.Project.Results.Model.NumberOfPanels
    End Sub

    Private Sub Calcular_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Calcular.Click

        Result = ""

        Dim Punto As New EVector3
        If Me.EnPuntoGeneral.Checked Then
            Punto.X = XBox.Value
            Punto.Y = YBox.Value
            Punto.Z = ZBox.Value
        End If
        If Me.EnPuntoDeControl.Checked Then

            Dim PuntoSeleccionado As Integer = PuntoDeControl.Value

            Dim Panel As Panel = MainForm.Project.Results.Model.Panel(PuntoSeleccionado)

            Punto.Assign(Panel.ControlPoint)

            Dim EsDelgado As String
            If Panel.IsSlender Then EsDelgado = "panel delgado" Else EsDelgado = "superficie sólida"
            Result = Result & String.Format("Punto de control: {0:D4} ({1})", PuntoSeleccionado, EsDelgado) & vbNewLine
            Result = Result & String.Format("Px = {0:F5}", Panel.ControlPoint.X) & vbNewLine
            Result = Result & String.Format("Py = {0:F5}", Panel.ControlPoint.Y) & vbNewLine
            Result = Result & String.Format("Pz = {0:F5}", Panel.ControlPoint.Z) & vbNewLine
            Result = Result & "Vector normal:" & vbNewLine
            Result = Result & String.Format("nx = {0:F5}", Panel.NormalVector.X) & vbNewLine
            Result = Result & String.Format("ny = {0:F5}", Panel.NormalVector.Y) & vbNewLine
            Result = Result & String.Format("nz = {0:F5}", Panel.NormalVector.Z) & vbNewLine
            Result = Result & "Área:" & vbNewLine
            Result = Result & String.Format("A  = {0:F5}", Panel.Area) & vbNewLine
            Result = Result & "Circulación:" & vbNewLine
            Result = Result & String.Format("G  = {0:F8}", Panel.Circulation) & vbNewLine
            Result = Result & "Coeficiente de presión:" & vbNewLine
            Result = Result & String.Format("Cp = {0:F8}", Panel.Cp) & vbNewLine

        End If
        'Dim Velocidad As EVector3 = Modulo_De_Cálculo_General.CalcularVelocidadEn(Punto)

        Result = Result & "Velocidad local:" & vbNewLine
        'Result = Result & String.Format("V = {0:F4} m/s", Velocidad.NormaEuclidea) & vbNewLine
        'Result = Result & String.Format("Vx = {0:F4} m/s", Velocidad.X) & vbNewLine
        'Result = Result & String.Format("Vy = {0:F4} m/s", Velocidad.Y) & vbNewLine
        'Result = Result & String.Format("Vz = {0:F4} m/s", Velocidad.Z) & vbNewLine

        Me.Reporte.Text = Result

    End Sub

    Private Sub PuntoDeControl_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PuntoDeControl.ValueChanged
        MainForm.Project.Results.Model.SelectControlPoint(PuntoDeControl.Value)
        MainForm.Project.RepresentOnGL()
    End Sub

End Class