﻿'OpenVOGEL (openvogel.org)
'Open source software for aerodynamics
'Copyright (C) 2021 Guillermo Hazebrouck (guillermo.hazebrouck@openvogel.org)

'This program Is free software: you can redistribute it And/Or modify
'it under the terms Of the GNU General Public License As published by
'the Free Software Foundation, either version 3 Of the License, Or
'(at your option) any later version.

'This program Is distributed In the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty Of
'MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License For more details.

'You should have received a copy Of the GNU General Public License
'along with this program.  If Not, see < http:  //www.gnu.org/licenses/>.

Imports OpenVOGEL.AeroTools.CalculationModel.Settings
Imports OpenVOGEL.MathTools.Algebra.EuclideanSpace
Imports OpenVOGEL.DesignTools

Public Class FormHistogram

    Private _Histogram As AeroelasticHistogram
    Private _SimSteps As Integer
    Private _SimStep As Double
    Private _SimSpan As Double
    Private _Velocity As Vector3

    Public Sub New()

        InitializeComponent()

        DoubleBuffered = True

        _Histogram = DataStore.SimulationSettings.AeroelasticHistogram.Clone
        _SimStep = DataStore.SimulationSettings.Interval
        _SimSteps = DataStore.SimulationSettings.SimulationSteps
        _SimSpan = _SimStep * _SimSteps
        _Velocity = DataStore.SimulationSettings.StreamVelocity.Clone

        BindData()

        UpdateHistogram()

        AddHandler _Histogram.ValueChanged, AddressOf UpdateHistogram

    End Sub

    Public Sub BindData()

        If Not IsNothing(_Histogram) Then

            nudHyperDamping.DataBindings.Add("Value", _Histogram, "HyperDamping", False, DataSourceUpdateMode.OnPropertyChanged)
            nudHyperDamping.DecimalPlaces = 4
            nudHyperDamping.Increment = 0.01
            nudHyperDamping.Minimum = 0

            nudHyperdampingSpan.DataBindings.Add("Value", _Histogram, "HyperDampingSpan", False, DataSourceUpdateMode.OnPropertyChanged)
            nudHyperdampingSpan.DecimalPlaces = 4
            nudHyperdampingSpan.Increment = 0.01
            nudHyperdampingSpan.Minimum = 0

            nudNormalDamping.DataBindings.Add("Value", _Histogram, "NormalDamping", False, DataSourceUpdateMode.OnPropertyChanged)
            nudNormalDamping.DecimalPlaces = 4
            nudNormalDamping.Increment = 0.01
            nudNormalDamping.Minimum = 0

            nudGustSpan.DataBindings.Add("Value", _Histogram, "GustSpan", False, DataSourceUpdateMode.OnPropertyChanged)
            nudGustSpan.DecimalPlaces = 4
            nudGustSpan.Increment = 0.01
            nudGustSpan.Minimum = 0

            nudGustX.DataBindings.Add("Value", _Histogram, "GustX", False, DataSourceUpdateMode.OnPropertyChanged)
            nudGustX.Increment = 0.5
            nudGustX.DecimalPlaces = 1

            nudGustY.DataBindings.Add("Value", _Histogram, "GustY", False, DataSourceUpdateMode.OnPropertyChanged)
            nudGustY.Increment = 0.5
            nudGustY.DecimalPlaces = 1

            nudGustZ.DataBindings.Add("Value", _Histogram, "GustZ", False, DataSourceUpdateMode.OnPropertyChanged)
            nudGustZ.Increment = 0.5
            nudGustZ.DecimalPlaces = 1

        End If

    End Sub

    Private _GustX_max As Double
    Private _GustX_min As Double

    Private _GustY_max As Double
    Private _GustY_min As Double

    Private _GustZ_max As Double
    Private _GustZ_min As Double

    Private Sub UpdateHistogram()

        _Histogram.Generate(_Velocity, _SimStep, _SimSteps)

        _GustX_max = Math.Max(_Histogram.GustX + _Velocity.X, _Velocity.X)
        _GustX_min = Math.Min(_Histogram.GustX + _Velocity.X, _Velocity.X)

        _GustY_max = Math.Max(_Histogram.GustY + _Velocity.Y, _Velocity.Y)
        _GustY_min = Math.Min(_Histogram.GustY + _Velocity.Y, _Velocity.Y)

        _GustZ_max = Math.Max(_Histogram.GustZ + _Velocity.Z, _Velocity.Z)
        _GustZ_min = Math.Min(_Histogram.GustZ + _Velocity.Z, _Velocity.Z)

        Refresh()

    End Sub

    Private Sub Plot(obj As Object, e As PaintEventArgs) Handles pbPLot.Paint

        Dim g As Graphics = e.Graphics

        g.FillRectangle(Brushes.White, e.ClipRectangle)
        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

        Dim m1 As New Drawing2D.Matrix(1, 0, 0, -1, 0, e.ClipRectangle.Height)
        g.Transform = m1

        Dim margin As Integer = 15
        Dim o As New Point(margin, 3 * margin)
        Dim h As Integer = pbPLot.Height - margin - o.Y
        Dim w As Integer = pbPLot.Width - 2 * margin

        Dim nGrid As Integer = 11

        For i = 0 To nGrid

            Dim p0 As New Point(o.X, o.Y + i / nGrid * h)
            Dim p1 As New Point(o.X + w, p0.Y)
            g.DrawLine(Pens.Gainsboro, p0, p1)

            p0 = New Point(o.X + i / nGrid * w, o.Y)
            p1 = New Point(p0.X, o.Y + h)
            g.DrawLine(Pens.Gainsboro, p0, p1)

        Next

        Dim pa As New Point(0, 0)
        Dim pb As New Point(0, 0)

        Dim Gust_max As Double = Math.Max(_Histogram.GustX, Math.Max(_Histogram.GustY, _Histogram.GustZ))
        Dim green_Pen As New Pen(Brushes.LimeGreen, 2)
        Dim red_Pen As New Pen(Brushes.Red, 2)
        Dim magenta_Pen As New Pen(Brushes.DarkViolet, 2)

        Dim f As New Font("Segoe UI", 7)

        For i = 1 To _SimSteps - 1

            Dim step0 As StepData = _Histogram.State(i - 1)
            Dim step1 As StepData = _Histogram.State(i)

            Dim ta As Double = step0.Time / _SimSpan
            Dim tb As Double = step1.Time / _SimSpan

            pa.X = o.X + ta * w
            pb.X = o.X + tb * w

            ' Lines:

            If (_GustX_max > _GustX_min) Then
                pa.Y = ((step0.Velocity.X - _GustX_min) / Gust_max) * h + o.Y
                pb.Y = ((step1.Velocity.X - _GustX_min) / Gust_max) * h + o.Y
                g.DrawLine(green_Pen, pa, pb)
            End If

            If (_GustY_max > _GustY_min) Then
                pa.Y = ((step0.Velocity.Y - _GustY_min) / Gust_max) * h + o.Y
                pb.Y = ((step1.Velocity.Y - _GustY_min) / Gust_max) * h + o.Y
                g.DrawLine(red_Pen, pa, pb)
            End If

            If (_GustZ_max > _GustZ_min) Then
                pa.Y = ((step0.Velocity.Z - _GustZ_min) / Gust_max) * h + o.Y
                pb.Y = ((step1.Velocity.Z - _GustZ_min) / Gust_max) * h + o.Y
                g.DrawLine(magenta_Pen, pa, pb)
            End If

        Next

        For i = 0 To _SimSteps - 1

            Dim step0 As StepData = _Histogram.State(i)

            Dim ta As Double = step0.Time / _SimSpan

            pa.X = o.X + ta * w

            ' Markers:

            If (_GustX_max > _GustX_min) Then
                pa.Y = ((step0.Velocity.X - _GustX_min) / Gust_max) * h + o.Y
                Dim r As New Rectangle(pa.X - 2, pa.Y - 2, 4, 4)
                g.DrawEllipse(Pens.Black, r)
                g.FillEllipse(Brushes.White, r)
            End If

            If (_GustY_max > _GustY_min) Then
                pa.Y = ((step0.Velocity.Y - _GustY_min) / Gust_max) * h + o.Y
                Dim r As New Rectangle(pa.X - 2, pa.Y - 2, 4, 4)
                g.DrawEllipse(Pens.Black, r)
                g.FillEllipse(Brushes.White, r)
            End If

            If (_GustZ_max > _GustZ_min) Then
                pa.Y = ((step0.Velocity.Z - _GustZ_min) / Gust_max) * h + o.Y
                Dim r As New Rectangle(pa.X - 2, pa.Y - 2, 4, 4)
                g.DrawEllipse(Pens.Black, r)
                g.FillEllipse(Brushes.White, r)
            End If

        Next

        ' Hyper damping span strip:

        Dim rhd As New Rectangle(o.X, margin, _Histogram.HyperDampingSpan / _SimSpan * w, margin)
        g.FillRectangle(Brushes.SkyBlue, rhd)

        Dim s As String = String.Format("Hyper damping {0}s - ξ={1}", _Histogram.HyperDampingSpan, _Histogram.HyperDamping)
        Dim s_size As SizeF = g.MeasureString(s, f)

        Dim m2 As New Drawing2D.Matrix(1, 0, 0, 1, 0, 0)
        g.Transform = m2

        Dim p As Point

        If rhd.Width > s_size.Width Then
            p.Y = e.ClipRectangle.Height - 1.5 * margin - 0.5 * s_size.Height
            p.X = o.X + Math.Min(0.5 * (rhd.Width - s_size.Width), 0.5 * (w - s_size.Width))
        Else
            p.Y = h + 1.5 * margin - 0.5 * s_size.Height
            p.X = o.X
        End If

        g.DrawString(s, f, Brushes.Blue, p)

    End Sub
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click

        DataStore.SimulationSettings.AeroelasticHistogram = _Histogram.Clone()

    End Sub

End Class