Imports System.Drawing.Image
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class DIP
    Public Shared SobelMaskX(,) As Integer = {{1, 0, -1}, {2, 0, -2}, {1, 0, -1}}
    Public Shared SobelMaskY(,) As Integer = {{-1, -2, -1}, {0, 0, 0}, {1, 2, 1}}

    Public Shared CannyMaskX(,) As Integer = {{0, 0, 0}, {1, 0, -1}, {0, 0, 0}}
    Public Shared CannyMaskY(,) As Integer = {{0, 1, 0}, {0, 0, 0}, {0, -1, 0}}

    Public Shared PrewittMaskX(,) As Integer = {{1, 0, -1}, {2, 0, -2}, {1, 0, -1}}

    Public Structure BitMapDataInfo
        Public bmd As BitmapData
        Dim sc As IntPtr
        Dim str As Integer
        Dim wRGB As Integer
        Dim w As Integer
        Dim h As Integer
        Dim numBt As Integer
    End Structure

    Public Shared Function GrayValue(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer) As Integer
        Return (r + g + b) / 3
    End Function

    Public Shared Function LimitValue(ByVal input As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
        Dim output As Integer

        If input < min Then
            output = min
        ElseIf input > max Then
            output = max
        Else
            output = input
        End If

        Return output
    End Function

    Public Shared Function BinaryValue(ByVal input As Integer, ByVal threshold As Integer) As Integer
        Dim output As Integer

        If input < threshold Then
            output = 0
        Else
            output = 255
        End If

        Return output
    End Function

    Public Shared Function GetIndexR(ByVal stride As Integer, ByVal x As Integer, ByVal y As Integer)
        Dim index As Integer = (y * stride) + (x * 3) + 2

        Return index
    End Function

    Public Shared Function GetPixelByte(ByVal array() As Byte, ByVal stride As Integer, ByVal x As Integer, ByVal y As Integer, ByVal rgb As String)
        Dim value As Byte

        If rgb = "r" Then
            value = array((y * stride) + (x * 3) + 2)
        ElseIf rgb = "g" Then
            value = array((y * stride) + (x * 3) + 1)
        ElseIf rgb = "b" Then
            value = array((y * stride) + (x * 3))
        End If

        Return value
    End Function

    Public Shared Sub CreateHistogram(pic As PictureBox, ByVal bmapImg As Bitmap, ByVal threshold As Integer)
        Dim Histo(255) As Integer

        For j = 0 To bmapImg.Height - 1
            For i = 0 To bmapImg.Width - 1
                Histo(bmapImg.GetPixel(i, j).R) += 1
            Next
        Next

        Dim w As Integer = pic.Width
        Dim h As Integer = pic.Height
        Dim padW As Double = w * 0.01
        Dim padH As Double = h * 0.01

        Dim bmap As New Bitmap(w, h)
        Dim g As Graphics = Graphics.FromImage(bmap)

        Dim histoMax As Integer = 0
        Dim scaleX As Double
        Dim scaleY As Double

        For i = 0 To 255
            If Histo(i) > histoMax Then
                histoMax = Histo(i)
            End If
        Next

        scaleX = Histo.Length / (w - 2 * padW)
        scaleY = histoMax / (h - padH)

        For i = 0 To 255
            Dim x1 As Single = padW + i / scaleX
            Dim x2 As Single = padW + i / scaleX
            Dim y1 As Single = h
            Dim y2 As Single = h - (Histo(i) / scaleY)
            g.DrawLine(Pens.Black, x1, y1, x2, y2)
        Next

        If threshold = 300 Then

        Else
            Dim x1 As Single = padW + threshold / scaleX
            Dim x2 As Single = padW + threshold / scaleX
            Dim y1 As Single = h
            Dim y2 As Single = 0
            g.DrawLine(Pens.Red, x1, y1, x2, y2)
        End If

        pic.Image = bmap
    End Sub
    '////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Public Shared Function GrayScaleImage(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapGray As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim col As Color
        Dim r, g, b, k As Integer

        For j = 0 To bmapOri.Height - 1
            For i = 0 To bmapOri.Width - 1
                col = bmapOri.GetPixel(i, j)
                r = col.R
                g = col.G
                b = col.B
                k = (r + b + g) / 3

                bmapGray.SetPixel(i, j, Color.FromArgb(k, k, k))
            Next
        Next

        Return bmapGray
    End Function

    Public Shared Function BinaryImage(ByVal bmapOri As Bitmap, ByVal threshold As Integer) As Bitmap
        Dim bmapBin As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim k As Integer

        For j = 0 To bmapOri.Height - 1
            For i = 0 To bmapOri.Width - 1
                k = bmapOri.GetPixel(i, j).R
                If k >= threshold Then
                    k = 255
                ElseIf k < threshold Then
                    k = 0
                End If

                bmapBin.SetPixel(i, j, Color.FromArgb(k, k, k))
            Next
        Next

        Return bmapBin
    End Function

    Public Shared Function HysteresisBinImage(ByVal bmapOri As Bitmap, ByVal thresholdL As Integer, ByVal thresholdH As Integer) As Bitmap
        Dim bmapBin As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim k(8) As Integer

        For j = 0 To bmapOri.Height - 1
            For i = 0 To bmapOri.Width - 1
                k(4) = bmapOri.GetPixel(i, j).R
                If k(4) < thresholdL Then
                    k(4) = 0
                ElseIf k(4) > thresholdH Then
                    k(4) = 255
                End If

                bmapBin.SetPixel(i, j, Color.FromArgb(k(4), k(4), k(4)))
            Next
        Next

        For j = 0 To bmapOri.Height - 1
            For i = 0 To bmapOri.Width - 1
                k(4) = bmapOri.GetPixel(i, j).R
                If k(4) >= thresholdL And k(4) <= thresholdH Then
                    If i = 0 Or j = 0 Then
                        k(0) = 0
                    Else
                        k(0) = bmapBin.GetPixel(i - 1, j - 1).R
                    End If

                    If j = 0 Then
                        k(1) = 0
                    Else
                        k(1) = bmapBin.GetPixel(i, j - 1).R
                    End If

                    If i = bmapBin.Width - 1 Or j = 0 Then
                        k(2) = 0
                    Else
                        k(2) = bmapBin.GetPixel(i + 1, j - 1).R
                    End If

                    If i = 0 Then
                        k(3) = 0
                    Else
                        k(3) = bmapBin.GetPixel(i - 1, j).R
                    End If

                    If i = bmapBin.Width - 1 Then
                        k(5) = 0
                    Else
                        k(5) = bmapBin.GetPixel(i + 1, j).R
                    End If

                    If i = 0 Or j = bmapBin.Height - 1 Then
                        k(6) = 0
                    Else
                        k(6) = bmapBin.GetPixel(i - 1, j + 1).R
                    End If

                    If j + 1 > bmapBin.Height - 1 Then
                        k(7) = 0
                    Else
                        k(7) = bmapBin.GetPixel(i, j + 1).R
                    End If

                    If i = bmapBin.Width - 1 Or j = bmapBin.Height - 1 Then
                        k(8) = 0
                    Else
                        k(8) = bmapBin.GetPixel(i + 1, j + 1).R
                    End If

                    k(4) = 0
                    For m = 0 To 8
                        If k(m) > 0 Then
                            k(4) = 255
                            Exit For
                        End If
                    Next

                    bmapBin.SetPixel(i, j, Color.FromArgb(k(4), k(4), k(4)))
                End If
            Next
        Next

        Return bmapBin
    End Function

    Public Shared Function OtsuThreshold(ByVal bmapOrigin As Bitmap) As Integer
        ' After Grayscale Process
        Dim bmapOtsu As New Bitmap(bmapOrigin.Width, bmapOrigin.Height)

        Dim Histo(255) As Integer
        Dim pxs As Integer = bmapOrigin.Width * bmapOrigin.Height

        Dim p(255) As Double

        Dim w1(255) As Double
        Dim w2(255) As Double

        Dim m1(255) As Double
        Dim m2(255) As Double

        Dim m1_m(255) As Double
        Dim m2_m(255) As Double

        Dim var1(255) As Double
        Dim var2(255) As Double

        Dim var1_m(255) As Double
        Dim var2_m(255) As Double

        Dim varw(255) As Double

        Dim thresData As Double
        Dim threshold As Integer

        For j = 0 To bmapOrigin.Height - 1
            For i = 0 To bmapOrigin.Width - 1
                Histo(bmapOrigin.GetPixel(i, j).R) += 1
            Next
        Next
        ''###################################################################################################
        ' Calculate p
        For i = 0 To 255
            p(i) = Histo(i) / pxs
        Next

        ' Calculate w1, w2  '' p(i) 누적값
        For t = 0 To 255
            For i = 0 To t - 1
                w1(t) += p(i)
            Next
            For i = t To 255
                w2(t) += p(i)
            Next
        Next

        ' Calculate m1, m2  '' 
        For t = 0 To 255
            For i = 0 To t - 1
                m1_m(t) += i * p(i)
            Next
            For i = t To 255
                m2_m(t) += i * p(i)
            Next

            m1(t) = m1_m(t) / w1(t)
            m2(t) = m2_m(t) / w2(t)
        Next

        ' Calculate var1, var2 (Variance)
        For t = 0 To 255
            For i = 0 To t - 1
                var1_m(t) += ((i - m1(t)) ^ 2) * p(i)
            Next
            For i = t To 255
                var2_m(t) += ((i - m2(t)) ^ 2) * p(i)
            Next

            var1(t) = var1_m(t) / w1(t)
            var2(t) = var2_m(t) / w2(t)
        Next

        ' Calculate varw
        For t = 0 To 255
            varw(t) = w1(t) * (var1(t) ^ 2) + w2(t) * (var2(t) ^ 2)
        Next

        ' Get Minimum Variance Within And Threshold
        thresData = varw(1)
        threshold = 1
        For i = 1 To 255
            If thresData > varw(i) Then
                thresData = varw(i)
                threshold = i
            End If
        Next

        Return threshold
    End Function
    '////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Public Shared Function EndInSearchImage(ByVal bmapOrigin As Bitmap, ByVal ratio As Single)
        Dim bmapEIS As New Bitmap(bmapOrigin.Width, bmapOrigin.Height)
        Dim col As Color
        Dim k As Integer

        Dim Histo(255) As Integer
        Dim LUT_EIS(255) As Integer
        Dim pxs As Integer = bmapOrigin.Width * bmapOrigin.Height
        Dim runsum As Integer
        Dim thresL, thresH As Integer

        For j = 0 To bmapOrigin.Height - 1
            For i = 0 To bmapOrigin.Width - 1
                Histo(bmapOrigin.GetPixel(i, j).R) += 1
            Next
        Next

        runsum = 0
        For i = 0 To 255
            runsum += Histo(i)
            If runsum / pxs >= ratio Then
                thresL = i
                Exit For
            End If
        Next

        runsum = 0
        For i = 255 To 0 Step -1
            runsum += Histo(i)
            If runsum / pxs >= ratio Then
                thresH = i
                Exit For
            End If
        Next

        For i = 0 To 255
            If i <= thresL Then
                LUT_EIS(i) = 0
            ElseIf i > thresL And i < thresH Then
                LUT_EIS(i) = 255 * (i - thresL) / (thresH - thresL)
            ElseIf i >= thresH Then
                LUT_EIS(i) = 255
            End If
        Next

        For j = 0 To bmapEIS.Height - 1
            For i = 0 To bmapEIS.Width - 1
                col = bmapOrigin.GetPixel(i, j)
                k = LUT_EIS(col.R)
                bmapEIS.SetPixel(i, j, Color.FromArgb(k, k, k))
            Next
        Next

        Return bmapEIS
    End Function

    Public Shared Function EqualizationImage(ByVal bmapOrigin As Bitmap)
        Dim bmapEq As New Bitmap(bmapOrigin.Width, bmapOrigin.Height)
        Dim col As Color
        Dim k As Integer

        Dim Histo(255) As Integer
        Dim cdf(255) As Single
        Dim LUT_Eq(255) As Integer
        Dim pxs As Integer = bmapOrigin.Height * bmapOrigin.Width

        For j = 0 To bmapOrigin.Height - 1
            For i = 0 To bmapOrigin.Width - 1
                Histo(bmapOrigin.GetPixel(i, j).R) += 1
            Next
        Next

        cdf(0) = Histo(0) / pxs
        For i = 1 To 255
            cdf(i) = cdf(i - 1) + Histo(i) / pxs
        Next

        For i = 0 To 255
            LUT_Eq(i) = CInt(cdf(i) * 255)
        Next

        For j = 0 To bmapEq.Height - 1
            For i = 0 To bmapEq.Width - 1
                col = bmapOrigin.GetPixel(i, j)
                k = col.R
                bmapEq.SetPixel(i, j, Color.FromArgb(LUT_Eq(k), LUT_Eq(k), LUT_Eq(k)))
            Next
        Next

        Return bmapEq
    End Function

    ' Uncompleted
    Public Shared Function HistogramMatching(ByVal bmapOrigin As Bitmap, ByVal bmapRef As Bitmap)
        Dim bmapMatch As New Bitmap(bmapOrigin.Width, bmapOrigin.Height)

        Dim col As Color
        Dim k As Integer

        Dim pxs1 As Integer = bmapOrigin.Width * bmapOrigin.Height
        Dim Histo1(255) As Integer
        Dim p1(255) As Double
        Dim CDF1(255) As Double
        Dim LUT1(255) As Integer

        Dim pxs2 As Integer = bmapRef.Width * bmapRef.Height
        Dim Histo2(255) As Integer
        Dim p2(255) As Double
        Dim CDF2(255) As Double
        Dim LUT2(255) As Integer

        Dim LUT3(255) As Integer

        ' Calc Histogram
        For j = 0 To bmapOrigin.Height - 1
            For i = 0 To bmapOrigin.Width - 1
                Histo1(bmapOrigin.GetPixel(i, j).R) += 1
            Next
        Next

        For j = 0 To bmapRef.Height - 1
            For i = 0 To bmapRef.Width - 1
                Histo2(bmapRef.GetPixel(i, j).R) += 1
            Next
        Next

        ' Calc p, CDF(Cumulative Distribution Function)
        For i = 0 To 255
            p1(i) = Histo1(i) / pxs1
            p2(i) = Histo2(i) / pxs2
            For j = 0 To i
                CDF1(i) += p1(j)
                CDF2(i) += p2(j)
            Next
        Next

        ' Calc Lookup Table
        For i = 0 To 255
            LUT1(i) = 255 * CDF1(i)
        Next

        For i = 0 To 255
            LUT2(i) = 255 * CDF2(i)
        Next

        Dim top As Integer
        Dim bottom As Integer
        For i = 255 To 1 Step -1
            top = LUT2(i)
            bottom = LUT2(i - 1)

            For j = bottom To top
                If top - LUT1(i) >= LUT1(i) - bottom Then
                    LUT3(j) = top
                Else
                    LUT3(j) = bottom
                End If
            Next
        Next

        For j = 0 To bmapMatch.Height - 1
            For i = 0 To bmapMatch.Width - 1
                col = bmapOrigin.GetPixel(i, j)
                k = col.R
                bmapMatch.SetPixel(i, j, Color.FromArgb(LUT3(k), LUT3(k), LUT3(k)))
            Next
        Next

        Return bmapMatch
    End Function
    '////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Public Shared Function Blurring33Image(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapBlur As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim r, g, b As Integer

        Dim col(8) As Color

        For j = 0 To bmapBlur.Height - 1
            For i = 0 To bmapBlur.Width - 1

                If i = 0 Or j = 0 Then
                    col(0) = Color.FromArgb(0, 0, 0)
                Else
                    col(0) = bmapOri.GetPixel(i - 1, j - 1)
                End If

                If j = 0 Then
                    col(1) = Color.FromArgb(0, 0, 0)
                Else
                    col(1) = bmapOri.GetPixel(i, j - 1)
                End If

                If i = bmapBlur.Width - 1 Or j = 0 Then
                    col(2) = Color.FromArgb(0, 0, 0)
                Else
                    col(2) = bmapOri.GetPixel(i + 1, j - 1)
                End If

                If i = 0 Then
                    col(3) = Color.FromArgb(0, 0, 0)
                Else
                    col(3) = bmapOri.GetPixel(i - 1, j)
                End If

                col(4) = bmapOri.GetPixel(i, j)

                If i = bmapBlur.Width - 1 Then
                    col(5) = Color.FromArgb(0, 0, 0)
                Else
                    col(5) = bmapOri.GetPixel(i + 1, j)
                End If


                If i = 0 Or j = bmapBlur.Height - 1 Then
                    col(6) = Color.FromArgb(0, 0, 0)
                Else
                    col(6) = bmapOri.GetPixel(i - 1, j + 1)
                End If

                If j + 1 > bmapBlur.Height - 1 Then
                    col(7) = Color.FromArgb(0, 0, 0)
                Else
                    col(7) = bmapOri.GetPixel(i, j + 1)
                End If

                If i = bmapBlur.Width - 1 Or j = bmapBlur.Height - 1 Then
                    col(8) = Color.FromArgb(0, 0, 0)
                Else
                    col(8) = bmapOri.GetPixel(i + 1, j + 1)
                End If

                r = 0
                g = 0
                b = 0

                For k = 0 To 8
                    r += col(k).R / 9
                    g += col(k).G / 9
                    b += col(k).B / 9
                Next

                bmapBlur.SetPixel(i, j, Color.FromArgb(r, g, b))
            Next
        Next

        Return bmapBlur
    End Function

    Public Shared Function Sharp33Image(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapSharp As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim r, g, b As Integer

        Dim col(8) As Color

        For j = 0 To bmapSharp.Height - 1
            For i = 0 To bmapSharp.Width - 1

                If i = 0 Or j = 0 Then
                    col(0) = Color.FromArgb(0, 0, 0)
                Else
                    col(0) = bmapOri.GetPixel(i - 1, j - 1)
                End If

                If j = 0 Then
                    col(1) = Color.FromArgb(0, 0, 0)
                Else
                    col(1) = bmapOri.GetPixel(i, j - 1)
                End If

                If i = bmapSharp.Width - 1 Or j = 0 Then
                    col(2) = Color.FromArgb(0, 0, 0)
                Else
                    col(2) = bmapOri.GetPixel(i + 1, j - 1)
                End If

                If i = 0 Then
                    col(3) = Color.FromArgb(0, 0, 0)
                Else
                    col(3) = bmapOri.GetPixel(i - 1, j)
                End If

                col(4) = bmapOri.GetPixel(i, j)

                If i = bmapSharp.Width - 1 Then
                    col(5) = Color.FromArgb(0, 0, 0)
                Else
                    col(5) = bmapOri.GetPixel(i + 1, j)
                End If


                If i = 0 Or j = bmapSharp.Height - 1 Then
                    col(6) = Color.FromArgb(0, 0, 0)
                Else
                    col(6) = bmapOri.GetPixel(i - 1, j + 1)
                End If

                If j + 1 > bmapSharp.Height - 1 Then
                    col(7) = Color.FromArgb(0, 0, 0)
                Else
                    col(7) = bmapOri.GetPixel(i, j + 1)
                End If

                If i = bmapSharp.Width - 1 Or j = bmapSharp.Height - 1 Then
                    col(8) = Color.FromArgb(0, 0, 0)
                Else
                    col(8) = bmapOri.GetPixel(i + 1, j + 1)
                End If

                r = col(4).R * 9
                g = col(4).G * 9
                b = col(4).B * 9
                For k = 0 To 8
                    If Not k = 4 Then
                        r -= col(k).R
                        g -= col(k).G
                        b -= col(k).B
                    End If
                Next
                r = r / 9
                g = g / 9
                b = b / 9

                If r < 0 Then
                    r = 0
                ElseIf r > 255 Then
                    r = 255
                End If

                If g < 0 Then
                    g = 0
                ElseIf g > 255 Then
                    g = 255
                End If

                If b < 0 Then
                    b = 0
                ElseIf b > 255 Then
                    b = 255
                End If

                bmapSharp.SetPixel(i, j, Color.FromArgb(r, g, b))
            Next
        Next

        Return bmapSharp
    End Function

    Public Shared Function GaussianSmoothingImage(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapGaus As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim k(8) As Double
        Dim bl As Double

        For j = 1 To bmapGaus.Height - 2
            For i = 1 To bmapGaus.Width - 2
                k(0) = bmapOri.GetPixel(i - 1, j - 1).R
                k(1) = bmapOri.GetPixel(i, j - 1).R
                k(2) = bmapOri.GetPixel(i + 1, j - 1).R

                k(3) = bmapOri.GetPixel(i - 1, j).R
                k(4) = bmapOri.GetPixel(i, j).R
                k(5) = bmapOri.GetPixel(i + 1, j).R

                k(6) = bmapOri.GetPixel(i - 1, j + 1).R
                k(7) = bmapOri.GetPixel(i, j + 1).R
                k(8) = bmapOri.GetPixel(i + 1, j + 1).R

                bl = k(0) / 16 + k(1) / 8 + k(2) / 16 + k(3) / 8 + k(4) / 4 + k(5) / 8 + k(6) / 16 + k(7) / 8 + k(8) / 16

                bmapGaus.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapGaus
    End Function

    Public Shared Function GaussianFunc(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapGaus As New Bitmap(bmapOri.Width, bmapOri.Height)

        Dim Gaussian1 As Double
        Dim Gaussian2 As Double
        Dim Sigma As Double
        Dim valLeft As Double
        Dim valRight As Double
        Dim valMid As Double
        Dim Gap As Double

        Dim gausMask(4, 4) As Double
        Dim sum As Double

        Dim gausK(bmapGaus.Width - 1, bmapGaus.Height - 1) As Integer
        Dim oriK(bmapGaus.Width - 1, bmapGaus.Height - 1) As Integer

        For i = 0 To 100
            Gaussian1 = Math.Exp(-4 / (0.1 * i + 0.1) ^ 2) / (2 * Math.PI * (0.1 * i + 0.1) ^ 2)
            Gaussian2 = Math.Exp(-4 / (0.1 * i + 0.2) ^ 2) / (2 * Math.PI * (0.1 * i + 0.2) ^ 2)

            If ((Gaussian1 - 0.0005) * (Gaussian2 - 0.0005)) <= 0 Then
                Sigma = 0.1 * i + 0.1
            End If
        Next

        valLeft = Sigma
        valRight = Sigma + 0.1

        Gap = valRight - valLeft

        While (Gap >= 0.001)
            valMid = (valLeft + valRight) / 2

            If ((Math.Exp(-4 / valLeft ^ 2) / (2 * Math.PI * valLeft ^ 2) - 0.0005) *
                (Math.Exp(-4 / valMid ^ 2) / (2 * Math.PI * valMid ^ 2) - 0.0005)) <= 0 Then
                valRight = valMid
            Else
                valLeft = valMid
            End If

            Gap = valRight - valLeft
            Sigma = valLeft
        End While

        For i = 0 To 4
            For j = 0 To 4
                gausMask(i, j) = Math.Exp(-((i - 2) ^ 2 + (j - 2) ^ 2) / (2 * Sigma ^ 2)) / (2 * Math.PI * Sigma ^ 2)
                sum = sum + gausMask(i, j)
            Next
        Next

        For j = 2 To bmapGaus.Height - 3
            For i = 2 To bmapGaus.Width - 3
                gausK(i, j) = 0
                oriK(i, j) = bmapOri.GetPixel(i, j).R
                For k = 0 To 4
                    For l = 0 To 4
                        gausK(i, j) += oriK(i + k - 2, j + l - 2) * gausMask(k, l)
                    Next
                Next
                bmapGaus.SetPixel(i, j, Color.FromArgb(gausK(i, j), gausK(i, j), gausK(i, j)))
            Next
        Next

        Return bmapGaus
    End Function
    '////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Public Shared Function HomoEdgeImage(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapEdge As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim k(8) As Integer
        Dim bl As Integer
        Dim diff As Integer

        For j = 1 To bmapEdge.Height - 2
            For i = 1 To bmapEdge.Width - 2
                k(0) = bmapOri.GetPixel(i - 1, j - 1).R
                k(1) = bmapOri.GetPixel(i, j - 1).R
                k(2) = bmapOri.GetPixel(i + 1, j - 1).R
                k(3) = bmapOri.GetPixel(i - 1, j).R

                k(4) = bmapOri.GetPixel(i, j).R

                k(5) = bmapOri.GetPixel(i + 1, j).R
                k(6) = bmapOri.GetPixel(i - 1, j + 1).R
                k(7) = bmapOri.GetPixel(i, j + 1).R
                k(8) = bmapOri.GetPixel(i + 1, j + 1).R

                diff = 0
                bl = 0
                For m = 0 To 8
                    diff = Math.Abs(k(m) - k(4))
                    If bl < diff Then
                        bl = diff
                    End If
                Next

                bmapEdge.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapEdge
    End Function

    Public Shared Function DiffEdgeImage(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapEdge As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim k(8) As Integer
        Dim bl As Integer
        Dim diff As Integer

        For j = 1 To bmapEdge.Height - 2
            For i = 1 To bmapEdge.Width - 2
                k(0) = bmapOri.GetPixel(i - 1, j - 1).R
                k(1) = bmapOri.GetPixel(i, j - 1).R
                k(2) = bmapOri.GetPixel(i + 1, j - 1).R
                k(3) = bmapOri.GetPixel(i - 1, j).R

                k(5) = bmapOri.GetPixel(i + 1, j).R
                k(6) = bmapOri.GetPixel(i - 1, j + 1).R
                k(7) = bmapOri.GetPixel(i, j + 1).R
                k(8) = bmapOri.GetPixel(i + 1, j + 1).R

                diff = 0
                bl = 0
                For m = 0 To 3
                    diff = Math.Abs(k(m) - k(8 - m))
                    If bl < diff Then
                        bl = diff
                    End If
                Next

                bmapEdge.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapEdge
    End Function

    Public Shared Function CannyEdgeImage(ByVal bmapOri As Bitmap) As Bitmap
        Dim bmapEdge As New Bitmap(bmapOri.Width, bmapOri.Height)

        Dim v(1) As Integer
        Dim h(1) As Integer

        Dim Ex(bmapOri.Width, bmapOri.Height) As Integer
        Dim Ey(bmapOri.Width, bmapOri.Height) As Integer

        Dim Es(bmapOri.Width, bmapOri.Height) As Double
        Dim Eo(bmapOri.Width, bmapOri.Height) As Double

        Dim Dk(bmapOri.Width, bmapOri.Height) As Double

        Dim Inm(bmapOri.Width, bmapOri.Height) As Double

        Dim D1 As Double
        Dim D2 As Double
        Dim D3 As Double
        Dim D4 As Double

        D1 = 0
        D2 = 45
        D3 = 90
        D4 = 135

        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                h(0) = bmapOri.GetPixel(i - 1, j).R
                h(1) = bmapOri.GetPixel(i + 1, j).R

                v(0) = bmapOri.GetPixel(i, j - 1).R
                v(1) = bmapOri.GetPixel(i, j + 1).R

                Ex(i, j) = h(0) - h(1)
                Ey(i, j) = v(0) - v(1)

                Es(i, j) = Math.Sqrt(Ex(i, j) * Ex(i, j) + Ey(i, j) * Ey(i, j))
                Eo(i, j) = Math.Atan2(Ey(i, j), Ex(i, j)) * 180 / Math.PI

                If Eo(i, j) < 0 Then
                    Eo(i, j) = 360 + Eo(i, j)
                End If

                If Eo(i, j) > 337.5 Or Eo(i, j) <= 22.5 Then        ' Degree 0
                    If Es(i, j) < Es(i, j - 1) Or Es(i, j) < Es(i, j + 1) Then
                        Es(i, j) = 0
                    End If
                ElseIf Eo(i, j) > 157.5 And Eo(i, j) <= 202.5 Then  ' Degree 180
                    If Es(i, j) < Es(i, j - 1) Or Es(i, j) < Es(i, j + 1) Then
                        Es(i, j) = 0
                    End If

                ElseIf Eo(i, j) > 22.5 And Eo(i, j) <= 67.5 Then    ' Degree 45
                    If Es(i, j) < Es(i - 1, j + 1) Or Es(i, j) < Es(i + 1, j - 1) Then
                        Es(i, j) = 0
                    End If
                ElseIf Eo(i, j) > 202.5 And Eo(i, j) <= 247.5 Then  ' Degree 225
                    If Es(i, j) < Es(i - 1, j + 1) Or Es(i, j) < Es(i + 1, j - 1) Then
                        Es(i, j) = 0
                    End If

                ElseIf Eo(i, j) > 67.5 And Eo(i, j) <= 112.5 Then   ' Degree 90
                    If Es(i, j) < Es(i - 1, j) Or Es(i, j) < Es(i + 1, j) Then
                        Es(i, j) = 0
                    End If
                ElseIf Eo(i, j) > 247.5 And Eo(i, j) <= 292.5 Then  ' Degree 270
                    If Es(i, j) < Es(i - 1, j) Or Es(i, j) < Es(i + 1, j) Then
                        Es(i, j) = 0
                    End If
                ElseIf Eo(i, j) > 112.5 And Eo(i, j) <= 157.5 Then  ' Degree 135
                    If Es(i, j) < Es(i + 1, j + 1) Or Es(i, j) < Es(i - 1, j - 1) Then
                        Es(i, j) = 0
                    End If
                ElseIf Eo(i, j) > 292.5 And Eo(i, j) <= 337.5 Then  ' Degree 315
                    If Es(i, j) < Es(i + 1, j + 1) Or Es(i, j) < Es(i - 1, j - 1) Then
                        Es(i, j) = 0
                    End If
                End If

                If Es(i, j) > 255 Then
                    Es(i, j) = 255
                End If

                bmapEdge.SetPixel(i, j, Color.FromArgb(Es(i, j), Es(i, j), Es(i, j)))
            Next
        Next

        Return bmapEdge
    End Function

    Public Shared Function RobertEdgeImage(ByVal bmapOri As Bitmap)
        Dim bmapEdge As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim bl As Integer

        Dim Px(1) As Integer
        Dim Py(1) As Integer

        Dim Gx(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gy(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gs(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Go(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double

        Dim top As Double

        For j = 1 To bmapEdge.Height - 2
            For i = 1 To bmapEdge.Width - 2
                Px(0) = bmapOri.GetPixel(i - 1, j - 1).R
                Px(1) = bmapOri.GetPixel(i + 1, j + 1).R
                Py(0) = bmapOri.GetPixel(i + 1, j - 1).R
                Py(1) = bmapOri.GetPixel(i - 1, j + 1).R

                Gx(i, j) = Px(0) - Px(1)
                Gy(i, j) = Py(0) - Py(1)
                Gs(i, j) = Math.Sqrt(Gx(i, j) * Gx(i, j) + Gy(i, j) * Gy(i, j))
                Go(i, j) = Math.Atan(Gx(i, j) / Gy(i, j))

                If Go(i, j) < 0 Then
                    Go(i, j) = 360 + Go(i, j)
                End If

                If Gs(i, j) > top Then
                    top = Gs(i, j)
                End If
            Next
        Next

        ' Non Maxima Suppression
        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                If (Go(i, j) > 337.5 Or Go(i, j) <= 22.5) Or (Go(i, j) > 157.5 And Go(i, j) <= 202.5) Then
                    ' Degree 0, Degree 180
                    If Gs(i, j) < Gs(i - 1, j) Or Gs(i, j) < Gs(i + 1, j) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 22.5 And Go(i, j) <= 67.5) Or (Go(i, j) > 202.5 And Go(i, j) <= 247.5) Then
                    ' Degree 45, Degree 225
                    If Gs(i, j) < Gs(i + 1, j + 1) Or Gs(i, j) < Gs(i - 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 67.5 And Go(i, j) <= 112.5) Or (Go(i, j) > 247.5 And Go(i, j) <= 292.5) Then
                    ' Degree 90, Degree 270
                    If Gs(i, j) < Gs(i, j - 1) Or Gs(i, j) < Gs(i, j + 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 112.5 And Go(i, j) <= 157.5) Or (Go(i, j) > 292.5 And Go(i, j) <= 337.5) Then
                    ' Degree 135, Degree 315
                    If Gs(i, j) < Gs(i - 1, j + 1) Or Gs(i, j) < Gs(i + 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                End If

                If Gs(i, j) > 255 Then
                    bl = 255
                Else
                    bl = Gs(i, j)
                End If

                bmapEdge.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapEdge
    End Function

    Public Shared Function SobelEdgeImage(ByVal bmapOri As Bitmap)
        Dim bmapEdge As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim bl As Integer

        Dim Px(5) As Integer
        Dim Py(5) As Integer

        Dim Gx(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gy(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gs(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Go(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double

        Dim top As Double

        For j = 1 To bmapEdge.Height - 2
            For i = 1 To bmapEdge.Width - 2
                Px(0) = bmapOri.GetPixel(i - 1, j - 1).R
                Px(1) = bmapOri.GetPixel(i - 1, j).R
                Px(2) = bmapOri.GetPixel(i - 1, j + 1).R
                Px(3) = bmapOri.GetPixel(i + 1, j - 1).R
                Px(4) = bmapOri.GetPixel(i + 1, j).R
                Px(5) = bmapOri.GetPixel(i + 1, j + 1).R

                Py(0) = bmapOri.GetPixel(i - 1, j - 1).R
                Py(1) = bmapOri.GetPixel(i, j - 1).R
                Py(2) = bmapOri.GetPixel(i + 1, j - 1).R
                Py(3) = bmapOri.GetPixel(i - 1, j + 1).R
                Py(4) = bmapOri.GetPixel(i, j + 1).R
                Py(5) = bmapOri.GetPixel(i + 1, j + 1).R

                Gx(i, j) = (Px(3) + 2 * Px(4) + Px(5)) - (Px(0) + 2 * Px(1) + Px(2))
                Gy(i, j) = (Py(3) + 2 * Py(4) + Py(5)) - (Py(0) + 2 * Py(1) + Py(2))
                Gs(i, j) = Math.Sqrt(Gx(i, j) * Gx(i, j) + Gy(i, j) * Gy(i, j))
                Go(i, j) = Math.Atan2(Gy(i, j), Gx(i, j)) * 180 / Math.PI

                If Go(i, j) < 0 Then
                    Go(i, j) = 360 + Go(i, j)
                End If

                If Gs(i, j) > top Then
                    top = Gs(i, j)
                End If
            Next
        Next

        ' Non Maxima Suppression
        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                If (Go(i, j) > 337.5 Or Go(i, j) <= 22.5) Or (Go(i, j) > 157.5 And Go(i, j) <= 202.5) Then
                    ' Degree 0, Degree 180
                    If Gs(i, j) < Gs(i - 1, j) Or Gs(i, j) < Gs(i + 1, j) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 22.5 And Go(i, j) <= 67.5) Or (Go(i, j) > 202.5 And Go(i, j) <= 247.5) Then
                    ' Degree 45, Degree 225
                    If Gs(i, j) < Gs(i + 1, j + 1) Or Gs(i, j) < Gs(i - 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 67.5 And Go(i, j) <= 112.5) Or (Go(i, j) > 247.5 And Go(i, j) <= 292.5) Then
                    ' Degree 90, Degree 270
                    If Gs(i, j) < Gs(i, j - 1) Or Gs(i, j) < Gs(i, j + 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 112.5 And Go(i, j) <= 157.5) Or (Go(i, j) > 292.5 And Go(i, j) <= 337.5) Then
                    ' Degree 135, Degree 315
                    If Gs(i, j) < Gs(i - 1, j + 1) Or Gs(i, j) < Gs(i + 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                End If

                If Gs(i, j) > 255 Then
                    bl = 255
                Else
                    bl = Gs(i, j)
                End If

                bmapEdge.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapEdge
    End Function

    Public Shared Function PrewittEdgeImage(ByVal bmapOri As Bitmap)
        Dim bmapEdge As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim bl As Integer

        Dim Px(5) As Integer
        Dim Py(5) As Integer

        Dim Gx(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gy(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gs(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Go(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double

        Dim top As Double

        For j = 1 To bmapEdge.Height - 2
            For i = 1 To bmapEdge.Width - 2
                Px(0) = bmapOri.GetPixel(i - 1, j - 1).R
                Px(1) = bmapOri.GetPixel(i - 1, j).R
                Px(2) = bmapOri.GetPixel(i - 1, j + 1).R
                Px(3) = bmapOri.GetPixel(i + 1, j - 1).R
                Px(4) = bmapOri.GetPixel(i + 1, j).R
                Px(5) = bmapOri.GetPixel(i + 1, j + 1).R

                Py(0) = bmapOri.GetPixel(i - 1, j - 1).R
                Py(1) = bmapOri.GetPixel(i, j - 1).R
                Py(2) = bmapOri.GetPixel(i + 1, j - 1).R
                Py(3) = bmapOri.GetPixel(i - 1, j + 1).R
                Py(4) = bmapOri.GetPixel(i, j + 1).R
                Py(5) = bmapOri.GetPixel(i + 1, j + 1).R

                Gx(i, j) = (Px(3) + Px(4) + Px(5)) - (Px(0) + Px(1) + Px(2))
                Gy(i, j) = (Py(3) + Py(4) + Py(5)) - (Py(0) + Py(1) + Py(2))
                Gs(i, j) = Math.Sqrt(Gx(i, j) * Gx(i, j) + Gy(i, j) * Gy(i, j))
                Go(i, j) = Math.Atan2(Gy(i, j), Gx(i, j)) * 180 / Math.PI

                If Go(i, j) < 0 Then
                    Go(i, j) = 360 + Go(i, j)
                End If

                If Gs(i, j) > top Then
                    top = Gs(i, j)
                End If
            Next
        Next

        ' Non Maxima Suppression
        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                If (Go(i, j) > 337.5 Or Go(i, j) <= 22.5) Or (Go(i, j) > 157.5 And Go(i, j) <= 202.5) Then
                    ' Degree 0, Degree 180
                    If Gs(i, j) < Gs(i - 1, j) Or Gs(i, j) < Gs(i + 1, j) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 22.5 And Go(i, j) <= 67.5) Or (Go(i, j) > 202.5 And Go(i, j) <= 247.5) Then
                    ' Degree 45, Degree 225
                    If Gs(i, j) < Gs(i + 1, j + 1) Or Gs(i, j) < Gs(i - 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 67.5 And Go(i, j) <= 112.5) Or (Go(i, j) > 247.5 And Go(i, j) <= 292.5) Then
                    ' Degree 90, Degree 270
                    If Gs(i, j) < Gs(i, j - 1) Or Gs(i, j) < Gs(i, j + 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 112.5 And Go(i, j) <= 157.5) Or (Go(i, j) > 292.5 And Go(i, j) <= 337.5) Then
                    ' Degree 135, Degree 315
                    If Gs(i, j) < Gs(i - 1, j + 1) Or Gs(i, j) < Gs(i + 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                End If

                If Gs(i, j) > 255 Then
                    bl = 255
                Else
                    bl = Gs(i, j)
                End If

                bmapEdge.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapEdge
    End Function

    Public Shared Function FreiChenEdgeImage(ByVal bmapOri As Bitmap)
        Dim bmapEdge As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim bl As Integer

        Dim Px(5) As Integer
        Dim Py(5) As Integer

        Dim Gx(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gy(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Gs(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double
        Dim Go(bmapEdge.Width - 1, bmapEdge.Height - 1) As Double

        Dim root2 As Double = Math.Sqrt(2)
        Dim top As Double

        For j = 1 To bmapEdge.Height - 2
            For i = 1 To bmapEdge.Width - 2
                Px(0) = bmapOri.GetPixel(i - 1, j - 1).R
                Px(1) = bmapOri.GetPixel(i - 1, j).R
                Px(2) = bmapOri.GetPixel(i - 1, j + 1).R
                Px(3) = bmapOri.GetPixel(i + 1, j - 1).R
                Px(4) = bmapOri.GetPixel(i + 1, j).R
                Px(5) = bmapOri.GetPixel(i + 1, j + 1).R

                Py(0) = bmapOri.GetPixel(i - 1, j - 1).R
                Py(1) = bmapOri.GetPixel(i, j - 1).R
                Py(2) = bmapOri.GetPixel(i + 1, j - 1).R
                Py(3) = bmapOri.GetPixel(i - 1, j + 1).R
                Py(4) = bmapOri.GetPixel(i, j + 1).R
                Py(5) = bmapOri.GetPixel(i + 1, j + 1).R

                Gx(i, j) = (Px(3) + root2 * Px(4) + Px(5)) - (Px(0) + root2 * Px(1) + Px(2))
                Gy(i, j) = (Py(3) + root2 * Py(4) + Py(5)) - (Py(0) + root2 * Py(1) + Py(2))
                Gs(i, j) = Math.Sqrt(Gx(i, j) * Gx(i, j) + Gy(i, j) * Gy(i, j))
                Go(i, j) = Math.Atan2(Gy(i, j), Gx(i, j)) * 180 / Math.PI

                If Go(i, j) < 0 Then
                    Go(i, j) = 360 + Go(i, j)
                End If

                If Gs(i, j) > top Then
                    top = Gs(i, j)
                End If
            Next
        Next

        ' Non Maxima Suppression
        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                If (Go(i, j) > 337.5 Or Go(i, j) <= 22.5) Or (Go(i, j) > 157.5 And Go(i, j) <= 202.5) Then
                    ' Degree 0, Degree 180
                    If Gs(i, j) < Gs(i - 1, j) Or Gs(i, j) < Gs(i + 1, j) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 22.5 And Go(i, j) <= 67.5) Or (Go(i, j) > 202.5 And Go(i, j) <= 247.5) Then
                    ' Degree 45, Degree 225
                    If Gs(i, j) < Gs(i + 1, j + 1) Or Gs(i, j) < Gs(i - 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 67.5 And Go(i, j) <= 112.5) Or (Go(i, j) > 247.5 And Go(i, j) <= 292.5) Then
                    ' Degree 90, Degree 270
                    If Gs(i, j) < Gs(i, j - 1) Or Gs(i, j) < Gs(i, j + 1) Then
                        Gs(i, j) = 0
                    End If
                ElseIf (Go(i, j) > 112.5 And Go(i, j) <= 157.5) Or (Go(i, j) > 292.5 And Go(i, j) <= 337.5) Then
                    ' Degree 135, Degree 315
                    If Gs(i, j) < Gs(i - 1, j + 1) Or Gs(i, j) < Gs(i + 1, j - 1) Then
                        Gs(i, j) = 0
                    End If
                End If

                If Gs(i, j) > 255 Then
                    bl = 255
                Else
                    bl = Gs(i, j)
                End If

                bmapEdge.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapEdge
    End Function
    '////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Public Shared Function HoughLineImage(ByVal bmapOri As Bitmap, ByVal threshold As Integer) As Bitmap
        Dim k As Integer

        Dim rMax As Integer = Math.Sqrt(Math.Pow(bmapOri.Width, 2) + Math.Pow(bmapOri.Height, 2))
        Dim tMax As Integer = 360

        Dim thetaR As Double
        Dim rho As Double

        Dim accLin(tMax, rMax) As Integer

        ' Voting Line algorithm
        For j = 0 To bmapOri.Height - 1
            For i = 0 To bmapOri.Width - 1
                k = bmapOri.GetPixel(i, j).R
                If k = 255 Then
                    For m = 0 To tMax           ' degree
                        thetaR = m * Math.PI / 180
                        rho = i * Math.Cos(thetaR) + j * Math.Sin(thetaR)
                        If rho <= 0 Or rho > rMax Then
                            Continue For
                        End If

                        accLin(m, rho) += 1
                    Next
                End If
            Next
        Next

        Dim bmapHough As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim g As Graphics
        Dim a As Double
        Dim b As Double
        Dim x0, y0, x1, y1 As Single

        bmapHough = bmapOri
        g = Graphics.FromImage(bmapHough)

        ' Drawing Detected Line
        For t = 0 To tMax           ' degree(theta)
            For r = 0 To rMax       ' rho   (rho)
                If accLin(t, r) > threshold And (Not t = 0) And (Not t = 180) Then
                    thetaR = t * Math.PI / 180
                    a = -Math.Cos(thetaR) / Math.Sin(thetaR)
                    b = r / Math.Sin(thetaR)

                    If (t >= 315 Or t < 45) Or (t >= 135 And t < 225) Then      ' Fix Y
                        y0 = 0
                        y1 = bmapHough.Height - 1

                        x0 = (y0 - b) / a
                        x1 = (y1 - b) / a

                        If x0 < 0 Then
                            x0 = 0
                            y0 = a * x0 + b
                        ElseIf x0 > bmapHough.Width - 1 Then
                            x0 = bmapHough.Width - 1
                            y0 = a * x0 + b
                        End If

                        If x1 < 0 Then
                            x1 = 0
                            y1 = a * x1 + b
                        ElseIf x1 > bmapHough.Width - 1 Then
                            x1 = bmapHough.Width - 1
                            y1 = a * x1 + b
                        End If

                    ElseIf (t >= 45 And t < 135) Or (t >= 225 And t < 315) Then  ' Fix X
                        x0 = 0
                        x1 = bmapHough.Width - 1

                        y0 = a * x0 + b
                        y1 = a * x1 + b

                        If y0 < 0 Then
                            y0 = 0
                            x0 = (y0 - b) / a
                        ElseIf y0 > bmapHough.Height - 1 Then
                            y0 = bmapHough.Height - 1
                            x0 = (y0 - b) / a
                        End If

                        If y1 < 0 Then
                            y1 = 0
                            x1 = (y1 - b) / a
                        ElseIf y1 > bmapHough.Height - 1 Then
                            y1 = bmapHough.Height - 1
                            x1 = (y1 - b) / a
                        End If
                    End If

                    g.DrawLine(Pens.Red, x0, y0, x1, y1)
                End If
            Next
        Next

        '=================================================================================================
        'Dim bmapPolar As New Bitmap(tMax, rMax)
        'Dim g As Graphics = Graphics.FromImage(bmapPolar)
        'For t = 0 To bmapPolar.Width - 1
        '    For r = 0 To bmapPolar.Height - 1
        '        If accLin(t, r) = 0 Then
        '            k = 255
        '        ElseIf accLin(t, r) > 255 Then
        '            k = 0
        '        Else
        '            k = 255 - accLin(t, r)
        '        End If

        '        bmapPolar.SetPixel(t, r, Color.FromArgb(k, k, k))
        '    Next
        'Next
        'picTrans1.Image = bmapPolar

        'Label2.Text = "라인 갯수: " + cnt.ToString
        '=================================================================================================

        Return bmapHough
    End Function

    Public Shared Function HoughCircleImage(ByVal bmapOri As Bitmap, ByVal rMin As Integer, ByVal threshold As Integer) As Bitmap
        Dim aMax As Integer = bmapOri.Width
        Dim bMax As Integer = bmapOri.Height
        Dim rMax As Integer = bmapOri.Width / 2
        Dim accCir(aMax, bMax, rMax) As Integer

        Dim rad As Double
        Dim a As Integer
        Dim b As Integer

        Dim LUTCS(1, 359, rMax) As Double

        ' Get Lookup Table r*cos(t), r*sin(t)
        For j = rMin To rMax                            ' radius
            For i = 0 To 359                            ' degree
                rad = i * Math.PI / 180
                LUTCS(0, i, j) = j * Math.Cos(rad)
                LUTCS(1, i, j) = j * Math.Sin(rad)
            Next
        Next

        ' Voting Circle algorithm 
        For j = 0 To bmapOri.Height - 1
            For i = 0 To bmapOri.Width - 1
                If bmapOri.GetPixel(i, j).R = 255 Then
                    For r = rMin To rMax                ' radius
                        For d = 0 To 359                ' degree
                            a = i - LUTCS(0, d, r)
                            b = j - LUTCS(1, d, r)
                            If a < 0 Or a > aMax Or b < 0 Or b > bMax Then
                                Continue For
                            End If
                            accCir(a, b, r) += 1
                        Next
                    Next
                End If
            Next
        Next

        Dim bmapHough As New Bitmap(bmapOri.Width, bmapOri.Height)
        bmapHough = bmapOri

        Dim x As Integer
        Dim y As Integer
        Dim dia As Integer
        Dim g As Graphics = Graphics.FromImage(bmapHough)

        ' Drawing Detected Circle
        For a = 0 To aMax                               ' center x
            For b = 0 To bMax                           ' center y
                For r = rMin To rMax                       ' radius
                    If accCir(a, b, r) > threshold Then
                        x = a - r
                        y = b - r
                        dia = 2 * r
                        g.DrawEllipse(Pens.Red, x, y, dia, dia)
                        g.DrawLine(Pens.Red, a - 10, b, a + 10, b)
                        g.DrawLine(Pens.Red, a, b - 10, a, b + 10)
                    End If
                Next
            Next
        Next

        ' Maximum Voted Circle
        'Dim max(3) As Integer
        'For a = 0 To aMax                               ' center x
        '    For b = 0 To bMax                           ' center y
        '        For r = rMin To rMax                       ' radius
        '            If max(0) < accCircle(a, b, r) Then
        '                max(0) = accCircle(a, b, r)
        '                max(1) = a
        '                max(2) = b
        '                max(3) = r
        '            End If
        '        Next
        '    Next
        'Next

        Return bmapHough
    End Function
    '////////////////////////////////////////////////////////////////////////////////////////////////////////////
    Public Shared Function CornerDetection(ByVal bmapOri As Bitmap, ByVal threshold As Integer) As Bitmap
        Dim bmapCorner As New Bitmap(bmapOri.Width, bmapOri.Height)
        bmapCorner = bmapOri

        Dim h(1) As Integer
        Dim v(1) As Integer

        Dim Ex(bmapOri.Width - 1, bmapOri.Height - 1) As Double
        Dim Ey(bmapOri.Width - 1, bmapOri.Height - 1) As Double

        Dim Ex2(bmapOri.Width - 1, bmapOri.Height - 1) As Double
        Dim Ey2(bmapOri.Width - 1, bmapOri.Height - 1) As Double

        Dim Exy(bmapOri.Width - 1, bmapOri.Height - 1) As Double

        Dim Ex2Sum As Double
        Dim Ey2Sum As Double
        Dim ExySum As Double

        Dim C(bmapOri.Width - 1, bmapOri.Height - 1, 2, 2) As Double

        Dim root As Double
        Dim r1(bmapOri.Width - 1, bmapOri.Height - 1) As Double
        Dim r2(bmapOri.Width - 1, bmapOri.Height - 1) As Double

        Dim detC(bmapOri.Width - 1, bmapOri.Height - 1) As Double
        Dim traC(bmapOri.Width - 1, bmapOri.Height - 1) As Double

        Dim corner(bmapOri.Width - 1, bmapOri.Height - 1) As Double

        Dim cornerTop As Double
        Dim cornerRatio As Double
        Dim cornerNormal(bmapOri.Width - 1, bmapOri.Height - 1) As Double

        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                h(0) = bmapOri.GetPixel(i - 1, j).R
                h(1) = bmapOri.GetPixel(i + 1, j).R

                v(0) = bmapOri.GetPixel(i, j - 1).R
                v(1) = bmapOri.GetPixel(i, j + 1).R

                Ex(i, j) = h(0) - h(1)
                Ey(i, j) = v(0) - v(1)

                Ex2(i, j) = Ex(i, j) * Ex(i, j)
                Ey2(i, j) = Ey(i, j) * Ey(i, j)

                Exy(i, j) = Ex(i, j) * Ey(i, j)
            Next
        Next

        cornerTop = 0
        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                Ex2Sum = 0
                Ey2Sum = 0
                ExySum = 0
                For k = -1 To 1
                    For l = -1 To 1
                        Ex2Sum += Ex2(i + l, j + k)
                        Ey2Sum += Ey2(i + l, j + k)
                        ExySum += Exy(i + l, j + k)
                    Next
                Next

                C(i, j, 0, 0) = Ex2Sum
                C(i, j, 1, 0) = ExySum
                C(i, j, 0, 1) = ExySum
                C(i, j, 1, 1) = Ey2Sum

                detC(i, j) = C(i, j, 0, 0) * C(i, j, 1, 1) - C(i, j, 1, 0) * C(i, j, 0, 1)
                traC(i, j) = C(i, j, 0, 0) + C(i, j, 1, 1)

                corner(i, j) = Math.Abs(detC(i, j) - 0.04 * traC(i, j) * traC(i, j))

                root = (Ex2Sum + Ey2Sum) * (Ex2Sum + Ey2Sum) - 4 * (ExySum * ExySum - Ex2Sum * Ey2Sum)
                If root > 0 Then
                    r1(i, j) = (-(Ex2Sum + Ey2Sum) + root) / 2
                    r2(i, j) = (-(Ex2Sum + Ey2Sum) - root) / 2
                End If

                If corner(i, j) > cornerTop Then
                    cornerTop = corner(i, j)
                End If
            Next
        Next

        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                If corner(i, j) > 0 Then

                End If
            Next
        Next

        Dim g As Graphics = Graphics.FromImage(bmapCorner)
        Dim bl As Integer
        cornerRatio = 255 / cornerTop
        For j = 1 To bmapOri.Height - 2
            For i = 1 To bmapOri.Width - 2
                cornerNormal(i, j) = corner(i, j) * cornerRatio
                bl = CInt(cornerNormal(i, j))
                If bl > threshold Then
                    g.DrawEllipse(Pens.Red, i - 5, j - 5, 10, 10)
                End If
            Next
        Next

        Return bmapCorner
    End Function

    Public Shared Function randomNoiseImage(ByVal bmapOri As Bitmap, ByVal percent As Double)
        Dim bmapNoise As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim pxs As Integer = bmapOri.Width * bmapOri.Height
        Dim noisePxs As Integer = pxs * percent

        Dim rand As New Random()
        Dim xRnd As Integer
        Dim yRnd As Integer

        For j = 0 To bmapNoise.Height - 1
            For i = 0 To bmapNoise.Width - 1
                bmapNoise.SetPixel(i, j, bmapOri.GetPixel(i, j))
            Next
        Next

        For i = 0 To noisePxs - 1
            xRnd = rand.Next(bmapNoise.Width)
            yRnd = rand.Next(bmapNoise.Height)
            bmapNoise.SetPixel(xRnd, yRnd, Color.FromArgb(255, 255, 255))
        Next

        Return bmapNoise
    End Function

    Public Shared Function MedianFilterImage(ByVal bmapOri As Bitmap)
        Dim bmapMF As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim sort(2, 8) As Integer

        For j = 1 To bmapMF.Height - 2
            For i = 1 To bmapMF.Width - 2

                For m = 0 To 2
                    If m = 0 Then
                        sort(m, 0) = bmapOri.GetPixel(i - 1, j - 1).R
                        sort(m, 1) = bmapOri.GetPixel(i, j - 1).R
                        sort(m, 2) = bmapOri.GetPixel(i + 1, j - 1).R

                        sort(m, 3) = bmapOri.GetPixel(i - 1, j).R
                        sort(m, 4) = bmapOri.GetPixel(i, j).R
                        sort(m, 5) = bmapOri.GetPixel(i + 1, j).R

                        sort(m, 6) = bmapOri.GetPixel(i - 1, j + 1).R
                        sort(m, 7) = bmapOri.GetPixel(i, j + 1).R
                        sort(m, 8) = bmapOri.GetPixel(i + 1, j + 1).R
                    ElseIf m = 1 Then
                        sort(m, 0) = bmapOri.GetPixel(i - 1, j - 1).G
                        sort(m, 1) = bmapOri.GetPixel(i, j - 1).G
                        sort(m, 2) = bmapOri.GetPixel(i + 1, j - 1).G

                        sort(m, 3) = bmapOri.GetPixel(i - 1, j).G
                        sort(m, 4) = bmapOri.GetPixel(i, j).G
                        sort(m, 5) = bmapOri.GetPixel(i + 1, j).G

                        sort(m, 6) = bmapOri.GetPixel(i - 1, j + 1).G
                        sort(m, 7) = bmapOri.GetPixel(i, j + 1).G
                        sort(m, 8) = bmapOri.GetPixel(i + 1, j + 1).G
                    ElseIf m = 2 Then
                        sort(m, 0) = bmapOri.GetPixel(i - 1, j - 1).B
                        sort(m, 1) = bmapOri.GetPixel(i, j - 1).B
                        sort(m, 2) = bmapOri.GetPixel(i + 1, j - 1).B

                        sort(m, 3) = bmapOri.GetPixel(i - 1, j).B
                        sort(m, 4) = bmapOri.GetPixel(i, j).B
                        sort(m, 5) = bmapOri.GetPixel(i + 1, j).B

                        sort(m, 6) = bmapOri.GetPixel(i - 1, j + 1).R
                        sort(m, 7) = bmapOri.GetPixel(i, j + 1).R
                        sort(m, 8) = bmapOri.GetPixel(i + 1, j + 1).R
                    End If

                    Dim temp As Integer
                    For k = 0 To 7
                        For l = k + 1 To 8
                            If sort(m, k) > sort(m, l) Then
                                temp = sort(m, k)
                                sort(m, k) = sort(m, l)
                                sort(m, l) = temp
                            End If
                        Next
                    Next
                Next

                bmapMF.SetPixel(i, j, Color.FromArgb(sort(0, 4), sort(1, 4), sort(2, 4)))
            Next
        Next

        Return bmapMF
    End Function

    Public Function DoGImage(ByVal bmapOri As Bitmap)
        Dim bmapDoG As New Bitmap(bmapOri.Width, bmapOri.Height)

        Dim sigma1 As Double
        Dim sigma2 As Double

        Dim maskDoG(6, 6) As Double

        Dim bl As Integer

        sigma1 = 0.8
        sigma2 = 1.6 * sigma1
        For j = 0 To 6
            For i = 0 To 6
                maskDoG(i, j) = Math.Exp(-(Math.Pow(i - 3, 2) + Math.Pow(j - 3, 2)) / (2 * Math.Pow(sigma1, 2))) /
                                (2 * Math.PI * Math.Pow(sigma1, 2)) -
                                Math.Exp(-(Math.Pow(i - 3, 2) + Math.Pow(j - 3, 2)) / (2 * Math.Pow(sigma2, 2))) /
                                (2 * Math.PI * Math.Pow(sigma2, 2))
            Next
        Next

        For j = 3 To bmapDoG.Height - 4
            For i = 3 To bmapDoG.Width - 4
                bl = 0
                For k = 0 To 6
                    For l = 0 To 6
                        bl += maskDoG(l, k) * bmapOri.GetPixel(i + l - 3, j + k - 3).R
                    Next
                Next


                bl += 60
                If bl < 0 Then
                    bl = 0
                ElseIf bl > 255 Then
                    bl = 255
                End If

                bmapDoG.SetPixel(i, j, Color.FromArgb(bl, bl, bl))
            Next
        Next

        Return bmapDoG
    End Function
End Class
