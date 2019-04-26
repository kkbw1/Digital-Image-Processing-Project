Imports System
Imports System.IO
Imports System.Drawing.Image
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class Form1
    Inherits System.Windows.Forms.Form

    Const WM_CAP As Short = &H400S

    Const WM_CAP_DRIVER_CONNECT As Integer = WM_CAP + 10
    Const WM_CAP_DRIVER_DISCONNECT As Integer = WM_CAP + 11
    Const WM_CAP_EDIT_COPY As Integer = WM_CAP + 30

    Public Const WM_CAP_GET_STATUS As Integer = WM_CAP + 54
    Public Const WM_CAP_DLG_VIDEOFORMAT As Integer = WM_CAP + 41

    Const WM_CAP_SET_PREVIEW As Integer = WM_CAP + 50
    Const WM_CAP_SET_PREVIEWRATE As Integer = WM_CAP + 52
    Const WM_CAP_SET_SCALE As Integer = WM_CAP + 53

    Const WS_CHILD As Integer = &H40000000
    Const WS_VISIBLE As Integer = &H10000000

    Const SWP_NOMOVE As Short = &H2S
    Const SWP_NOSIZE As Short = 1
    Const SWP_NOZORDER As Short = &H4S

    Const HWND_BOTTOM As Short = 1

    Dim iDevice As Integer = 0 ' Current device ID
    Dim hHwnd As Integer ' Handle to preview window

    Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
        (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, _
        <MarshalAs(UnmanagedType.AsAny)> ByVal lParam As Object) As Integer

    Declare Function SetWindowPos Lib "user32" Alias "SetWindowPos" (ByVal hwnd As Integer, _
        ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, _
        ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer

    Declare Function DestroyWindow Lib "user32" (ByVal hndw As Integer) As Boolean

    Declare Function capCreateCaptureWindowA Lib "avicap32.dll" _
        (ByVal lpszWindowName As String, ByVal dwStyle As Integer, _
        ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, _
        ByVal nHeight As Short, ByVal hWndParent As Integer, _
        ByVal nID As Integer) As Integer

    Declare Function capGetDriverDescriptionA Lib "avicap32.dll" (ByVal wDriver As Short, _
        ByVal lpszName As String, ByVal cbName As Integer, ByVal lpszVer As String, _
        ByVal cbVer As Integer) As Boolean
    '=====================================================================================================
    Dim data As IDataObject
    '====================================================================================================
    Dim gr As Graphics

    Dim rad337_5 As Double = -(360 - 337.5) / 180.0 * 3.14
    Dim rad22_5 As Double = 22.5 / 180.0 * 3.14

    Dim rad157_5 As Double = 157.5 / 180.0 * 3.14
    Dim rad202_5 As Double = -(360 - 202.5) / 180.0 * 3.14

    Dim rad67_5 As Double = 67.5 / 180.0 * 3.14

    Dim rad247_5 As Double = -(360 - 247.5) / 180.0 * 3.14

    Dim rad112_5 As Double = 112.5 / 180.0 * 3.14

    Dim rad292_5 As Double = -(360 - 292.5) / 180.0 * 3.14
    '=====================================================================================================
    Dim bmapSmplOri As Bitmap

    Private Sub Obstacle()
        '////////////////////////////////////////////////////////////////////////////////////////////////////////
        Dim bmapOb As New Bitmap(320, 240)
        data = Clipboard.GetDataObject()
        If data.GetDataPresent(GetType(System.Drawing.Bitmap)) Then
            bmapOb = CType(data.GetData(GetType(System.Drawing.Bitmap)), Image)
        End If

        'If (bmapMatch Is Nothing) Then
        '    Return
        'End If

        'If bmapSmplOri Is Nothing Then
        '    Return
        'End If

        Dim bmdOb As BitmapData = bmapOb.LockBits(New Rectangle(0, 0, bmapOb.Width, bmapOb.Height),
            ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)
        Dim scOb As IntPtr = bmdOb.Scan0
        Dim strOb As Integer = bmdOb.Stride
        Dim wOb As Integer = bmdOb.Width * 3
        Dim hOb As Integer = bmdOb.Height
        Dim btOb As Integer = Math.Abs(strOb) * hOb

        Dim pxsOb(btOb - 1) As Byte
        Dim pxsObK(btOb - 1) As Byte
        Dim pxsObKB(btOb - 1) As Byte
        Dim pxsObKE(btOb - 1) As Byte
        Dim pxsObKEB(btOb - 1) As Byte

        Dim pxsObDet(btOb - 1) As Byte

        Marshal.Copy(scOb, pxsOb, 0, pxsOb.Length)

        Dim idxR As Integer
        Dim idxG As Integer
        Dim idxB As Integer

        Dim r, g, b As Integer
        Dim k As Integer

        Dim offset As Integer

        Dim wPxOb = wOb / 3

        '////////////////////////////// Grayscale Process  //////////////////////////////////////////
        For j = 0 To hOb - 1
            For i = 0 To wPxOb - 1
                idxR = DIP.GetIndexR(strOb, i, j)
                idxG = idxR - 1
                idxB = idxR - 2

                r = pxsOb(idxR)
                g = pxsOb(idxG)
                b = pxsOb(idxB)

                k = (r + g + b) / 3

                pxsObK(idxR) = k
                pxsObK(idxG) = k
                pxsObK(idxB) = k

                k = (r * 7 + g * 2 + b * 2) / 11

                pxsObK(idxR) = k
                pxsObK(idxG) = k
                pxsObK(idxB) = k
            Next
        Next
        '/////////////////////////////////////////////////////////////////////////////////////////////////
        '////////////////////////////////////////// Edge Process /////////////////////////////////////////
        Dim Ex(wOb / 3, hOb) As Integer
        Dim Ey(wOb / 3, hOb) As Integer
        Dim Es(wOb / 3, hOb) As Double
        Dim Eo(wOb / 3, hOb) As Double
        offset = 1
        For j = offset To (hOb - offset) - 1
            For i = offset * 3 To wPxOb - offset - 1
                For m = 0 To offset * 2
                    For n = 0 To offset * 2
                        Dim temp As Integer = pxsObK(DIP.GetIndexR(strOb, i - 1 + n, j - 1 + m))
                        Ex(i, j) += temp * DIP.CannyMaskX(n, m)
                        Ey(j, j) += temp * DIP.CannyMaskY(n, m)
                    Next
                Next

                Es(i, j) = Math.Sqrt(Math.Pow(Ex(i, j), 2) + Math.Pow(Ey(i, j), 2))
                Eo(i, j) = Math.Atan2(Ey(i, j), Ex(i, j))

                idxR = DIP.GetIndexR(strOb, i, j)
                idxG = idxR - 1
                idxB = idxR - 2

                Dim temp1 As Integer = DIP.LimitValue(Es(i, j), 0, 255)
                pxsObKE(idxR) = temp1
                pxsObKE(idxG) = temp1
                pxsObKE(idxB) = temp1
                temp1 = DIP.BinaryValue(temp1, 50)
                pxsObKEB(idxR) = temp1
                pxsObKEB(idxG) = temp1
                pxsObKEB(idxB) = temp1

                Dim temp2 As Integer = DIP.BinaryValue(pxsObK(idxR), 100)
                pxsObKB(idxR) = temp2
                pxsObKB(idxG) = temp2
                pxsObKB(idxB) = temp2
            Next
        Next
        '/////////////////////////////////////////////////////////////////////////////////////////////////
        '////////////////////////////////////////// Obstacle Detect //////////////////////////////////////
        For i = 0 To pxsObKEB.Length - 1
            pxsObDet(i) = pxsObKEB(i)
        Next

        Dim low As Integer = 0
        For i = 0 To wPxOb - 1
            For j = hOb - hOb * 0.05 To 0 Step -1
                idxR = DIP.GetIndexR(strOb, i, j)
                idxG = idxR - 1
                idxB = idxR - 2

                If pxsObDet(idxR) = 255 Then
                    If low = 0 Then
                        low = j
                    End If
                    If Math.Abs(low - j) <= 20 Then
                        For m = 0 To hOb - 1
                            idxR = DIP.GetIndexR(strOb, i, m)
                            idxG = idxR - 1
                            idxB = idxR - 2
                            If m = j Then
                                pxsObDet(idxR) = 255
                                pxsObDet(idxG) = 255
                                pxsObDet(idxB) = 255
                            Else
                                pxsObDet(idxR) = 0
                                pxsObDet(idxG) = 0
                                pxsObDet(idxB) = 0
                            End If
                        Next
                    Else
                        For m = 0 To hOb - 1
                            pxsObDet(idxR) = 0
                            pxsObDet(idxG) = 0
                            pxsObDet(idxB) = 0
                        Next
                    End If
                End If
            Next
        Next

        Dim x_pre(wPxOb) As Integer
        Dim dist(wPxOb) As Integer
        Dim kk As Integer = 0
        For j = 0 To hOb - 1
            For i = 0 To wPxOb - 1
                idxR = DIP.GetIndexR(strOb, i, j)
                If pxsObDet(idxR) > 250 Then
                    x_pre(kk) = i
                    If kk <> 0 Then
                        dist(kk) = x_pre(kk) - x_pre(kk - 1)
                    End If
                    kk = kk + 1
                End If
            Next
        Next

        Dim wMax As Integer
        Dim xMax As Integer
        For i = 0 To wPxOb - 1
            If dist(i) > wMax Then
                wMax = dist(i)
                xMax = i
            End If
        Next

        '////////////////////////////////////////////////////////////////////////////////////////////////////
        Marshal.Copy(pxsOb, 0, scOb, pxsOb.Length)
        bmapOb.UnlockBits(bmdOb)

        gr = Graphics.FromImage(bmapOb)
        gr.DrawLine(New Pen(Color.Yellow, 4), xMax, 320, xMax, 0)

        picMatch.Image = bmapOb
    End Sub

    Private Sub Transform()
        Dim xR As Integer = 135
        Dim yR As Integer = 140
        Dim wR As Integer = 50
        Dim hR As Integer = 100

        Dim bmapOri As New Bitmap(320, 240)
        Dim bmapLaser As New Bitmap(wR, hR)
        Dim bmapTrans As New Bitmap(bmapOri.Width, bmapOri.Height)

        ' Get image from clipboard and convert it to a bitmap
        data = Clipboard.GetDataObject()
        If data.GetDataPresent(GetType(System.Drawing.Bitmap)) Then
            bmapOri = CType(data.GetData(GetType(System.Drawing.Bitmap)), Image)
        End If

        '=====================================================================================================
        Dim bmdOri As BitmapData = bmapOri.LockBits(New Rectangle(0, 0, bmapOri.Width, bmapOri.Height),
            ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        Dim scOri As IntPtr = bmdOri.Scan0
        Dim strOri As Integer = bmdOri.Stride
        Dim wOri As Integer = bmdOri.Width * 3
        Dim hOri As Integer = bmdOri.Height
        Dim btOri As Integer = Math.Abs(strOri) * hOri

        Dim pxsOri(btOri - 1) As Byte

        Dim pxsOriK(btOri - 1) As Byte
        Dim pxsOriKB(btOri - 1) As Byte
        Dim pxsOriKE(btOri - 1) As Byte
        Dim pxsOriKEB(btOri - 1) As Byte

        Dim pxsOriR(btOri - 1) As Byte

        Dim pxsOriOb(btOri - 1) As Byte

        Marshal.Copy(scOri, pxsOri, 0, pxsOri.Length)    'bmapOri => pxsOri Byte Array
        '///////////////////////////////////////////////////////////////////////////////////////////////////
        Dim bmdLaser As BitmapData = bmapLaser.LockBits(New Rectangle(0, 0, bmapLaser.Width, bmapLaser.Height),
            ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        Dim scLsr As IntPtr = bmdLaser.Scan0
        Dim strLsr As Integer = bmdLaser.Stride
        Dim wLsr As Integer = bmdLaser.Width * 3
        Dim hLsr As Integer = bmdLaser.Height
        Dim btLsr As Integer = Math.Abs(strLsr) * hLsr

        Dim pxsLsr(btLsr - 1) As Byte
        Dim pxsLsrK(btLsr - 1) As Byte
        Dim pxsLsrKB(btLsr - 1) As Byte
        Dim pxsLsrKE(btLsr - 1) As Byte
        Dim pxsLsrKEB(btLsr - 1) As Byte
        '///////////////////////////////////////////////////////////////////////////////////////////////////
        '        Dim bmdTrans As BitmapData = bmapTrans.LockBits(New Rectangle(0, 0, bmapTrans.Width, bmapTrans.Height),
        'ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        '        Dim scTrans As IntPtr = bmdTrans.Scan0
        '        Dim strTrans As Integer = bmdTrans.Stride
        '        Dim wTrans As Integer = bmdTrans.Width * 3
        '        Dim hTrans As Integer = bmdTrans.Height
        '        Dim btTrans As Integer = Math.Abs(strTrans) * hTrans

        '        Dim pxsTrans(btTrans - 1) As Byte
        '///////////////////////////////////////////////////////////////////////////////////////////////////
        Dim iPx As Integer
        Dim idxR As Integer
        Dim idxG As Integer
        Dim idxB As Integer

        Dim r, g, b As Integer
        Dim k As Integer

        Dim offset As Integer

        Dim wPxOri As Integer = wOri / 3
        Dim wPxLsr As Integer = wLsr / 3
        '/////////////////// Grayscale Process => Enrty Area '' Copy RGB Image => Laser Area ///////////////
        For j = 0 To hOri - 1
            For i = 0 To wPxOri - 1
                idxR = DIP.GetIndexR(strOri, i, j)
                idxG = idxR - 1
                idxB = idxR - 2

                r = pxsOri(idxR)
                g = pxsOri(idxG)
                b = pxsOri(idxB)

                k = (r + g + b) / 3

                pxsOriK(idxR) = k
                pxsOriK(idxG) = k
                pxsOriK(idxB) = k

                k = (r * 7 + g * 2 + b * 2) / 11

                pxsOriR(idxR) = k
                pxsOriR(idxG) = k
                pxsOriR(idxB) = k

                If i >= xR And i <= xR + wR - 1 Then
                    If j >= yR And j <= yR + hR - 1 Then
                        idxR = DIP.GetIndexR(strLsr, i - xR, j - yR)
                        idxG = idxR - 1
                        idxB = idxR - 2

                        pxsLsr(idxR) = r
                        pxsLsr(idxG) = g
                        pxsLsr(idxB) = b

                        pxsLsrK(idxR) = k
                        pxsLsrK(idxG) = k
                        pxsLsrK(idxB) = k
                    End If
                End If
            Next
        Next
        '//////////////////////////////////////////////////////////////////////////////////////////////
        '//////////////// GrayScale Median Filtering Process => Entry Area & Laser Area ///////////////
        'Dim sort(8) As Integer
        'offset = 1
        'For j = offset To hOri - offset - 1
        '    For i = (offset * 3) To (wOri - (offset * 3)) - 1 Step 3
        '        iPx = i / 3

        '        For m = 0 To 2
        '            For n = 0 To 2
        '                sort(m * 3 + n) = pxsOriK(DIP.GetIndexR(strOri, iPx - 1 + n, j - 1 + m))
        '            Next
        '        Next

        '        Dim temp As Integer
        '        For m = 0 To 7
        '            For n = m + 1 To 8
        '                If sort(m) > sort(n) Then
        '                    temp = sort(m)
        '                    sort(m) = sort(n)
        '                    sort(n) = temp
        '                End If
        '            Next
        '        Next

        '        idxR = DIP.GetIndexR(strOri, iPx, j)
        '        idxG = idxR - 1
        '        idxB = idxR - 2

        '        pxsOriK(idxR) = sort(4)
        '        pxsOriK(idxG) = sort(4)
        '        pxsOriK(idxB) = sort(4)

        '        If iPx >= xR And iPx <= xR + wR - 1 Then
        '            If j >= yR And j <= yR + hR - 1 Then
        '                idxR = DIP.GetIndexR(strLaser, iPx - xR, j - yR)
        '                idxG = idxR - 1
        '                idxB = idxR - 2

        '                pxsLsrK(idxR) = sort(4)
        '                pxsLsrK(idxG) = sort(4)
        '                pxsLsrK(idxB) = sort(4)
        '            End If
        '        End If
        '    Next
        'Next
        '//////////////////////////////////////////////////////////////////////////////////////////////////
        '///////////////////////// Sobel Mask Edge Process & Binary Process => Entry Area ///////////////////
        Dim Ex(wOri / 3, hOri) As Integer
        Dim Ey(wOri / 3, hOri) As Integer
        Dim Es(wOri / 3, hOri) As Double
        Dim Eo(wOri / 3, hOri) As Double
        offset = 1
        For j = offset To (hOri - offset) - 1
            For i = offset * 3 To wPxOri - offset - 1
                For m = 0 To offset * 2
                    For n = 0 To offset * 2
                        Dim temp As Integer = pxsOriK(DIP.GetIndexR(strOri, i - 1 + n, j - 1 + m))
                        Ex(i, j) += temp * DIP.CannyMaskX(n, m)
                        Ey(j, j) += temp * DIP.CannyMaskY(n, m)
                    Next
                Next

                Es(i, j) = Math.Sqrt(Math.Pow(Ex(i, j), 2) + Math.Pow(Ey(i, j), 2))
                Eo(i, j) = Math.Atan2(Ey(i, j), Ex(i, j))

                idxR = DIP.GetIndexR(strOri, i, j)
                idxG = idxR - 1
                idxB = idxR - 2

                Dim temp1 As Integer = DIP.LimitValue(Es(i, j), 0, 255)
                pxsOriKE(idxR) = temp1
                pxsOriKE(idxG) = temp1
                pxsOriKE(idxB) = temp1
                temp1 = DIP.BinaryValue(temp1, 50)
                pxsOriKEB(idxR) = temp1
                pxsOriKEB(idxG) = temp1
                pxsOriKEB(idxB) = temp1

                Dim temp2 As Integer = DIP.BinaryValue(pxsOriK(idxR), 100)
                pxsOriKB(idxR) = temp2
                pxsOriKB(idxG) = temp2
                pxsOriKB(idxB) = temp2

                If i >= xR And i <= xR + wR - 1 Then
                    If j >= yR And j <= yR + hR - 1 Then
                        idxR = DIP.GetIndexR(strLsr, i - xR, j - yR)
                        idxG = idxR - 1
                        idxB = idxR - 2

                        pxsLsrKE(idxR) = temp1
                        pxsLsrKE(idxG) = temp1
                        pxsLsrKE(idxB) = temp1

                        temp1 = DIP.BinaryValue(pxsLsrKE(idxR), 125)
                        pxsLsrKEB(idxR) = temp1
                        pxsLsrKEB(idxG) = temp1
                        pxsLsrKEB(idxB) = temp1

                        pxsLsrKB(idxR) = temp2
                        pxsLsrKB(idxG) = temp2
                        pxsLsrKB(idxB) = temp2
                    End If
                End If
            Next
        Next
        '//////////////////////////////////////////////////////////////////////////////////////
        '/////////////////////////// Non-Suppression Maxima /////////////////////////////////
        'offset = 1
        'For j = offset To (hOri - offset) - 1
        '    For i = offset * 3 To (wOri - offset * 3) - 1 Step 3
        '        iPx = i / 3
        '        idxR = DIP.GetIndexR(strOri, iPx, j)
        '        idxG = idxR - 1
        '        idxB = idxR - 2

        '        If (Eo(iPx, j) > rad337_5 Or Eo(iPx, j) <= rad22_5) Or
        '            (Eo(iPx, j) > rad157_5 And Eo(iPx, j) < rad202_5) Then
        '            If Es(iPx, j) < Es(iPx, j - 1) Or Es(iPx, j) < Es(iPx, j + 1) Then
        '                Es(iPx, j) = 0
        '            End If
        '        ElseIf (Eo(iPx, j) > rad67_5 And Eo(iPx, j) <= rad112_5) Or
        '            (Eo(iPx, j) > rad247_5 And Eo(iPx, j) <= rad292_5) Then
        '            If Es(iPx, j) < Es(iPx - 1, j) Or Es(iPx, j) < Es(iPx + 1, j) Then
        '                Es(iPx, j) = 0
        '            End If
        '        ElseIf (Eo(iPx, j) > rad22_5 And Eo(iPx, j) <= rad67_5) Or
        '            (Eo(iPx, j) > rad202_5 And Eo(iPx, j) <= rad247_5) Then
        '            If Es(iPx, j) < Es(iPx - 1, j - 1) Or Es(iPx, j) < Es(iPx + 1, j + 1) Then
        '                Es(iPx, j) = 0
        '            End If
        '        ElseIf (Eo(iPx, j) > rad112_5 And Eo(iPx, j) <= rad157_5) Or
        '            (Eo(iPx, j) > rad292_5 And Eo(iPx, j) <= rad337_5) Then
        '            If Es(iPx, j) < Es(iPx + 1, j - 1) Or Es(iPx, j) < Es(iPx - 1, j + 1) Then
        '                Es(iPx, j) = 0
        '            End If
        '        End If

        '        Dim temp As Integer = DIP.LimitValue(Es(iPx, j), 0, 255)
        '        pxsOriE(idxR) = temp
        '        pxsOriE(idxG) = temp
        '        pxsOriE(idxB) = temp

        '        If iPx >= xR And iPx <= xR + wR - 1 Then
        '            If j >= yR And j <= yR + hR - 1 Then
        '                idxR = DIP.GetIndexR(strLsr, iPx - xR, j - yR)
        '                idxG = idxR - 1
        '                idxB = idxR - 2

        '                pxsLsrE(idxR) = temp
        '                pxsLsrE(idxG) = temp
        '                pxsLsrE(idxB) = temp

        '                temp = DIP.BinaryValue(pxsLsrE(idxR), 125)
        '                pxsLsrEB(idxR) = temp
        '                pxsLsrEB(idxG) = temp
        '                pxsLsrEB(idxB) = temp
        '            End If
        '        End If
        '    Next
        'Next
        '//////////////////////////////////////////////////////////////////////////////////////
        '//////////////////////////////// Circle Hough Transform ////////////////////////////////////
        Dim degStep As Integer = 8
        Dim degSeg As Integer = 45
        Dim radius As Integer = 5
        'Dim vote(wR, hR, radius) As Integer
        Dim vote(wR, hR) As Integer
        Dim LUTCS(1, degStep, radius) As Double    '' cs, degree, radius
        For j = 1 To radius
            For i = 0 To degStep
                Dim rad As Double = (i * degSeg) * Math.PI / 180
                LUTCS(0, i, j) = j * Math.Cos(rad)
                LUTCS(1, i, j) = j * Math.Sin(rad)
            Next
        Next

        Dim aa As Integer
        Dim bb As Integer
        For j = 0 To hLsr - 1
            For i = 0 To wPxLsr - 1
                idxR = DIP.GetIndexR(strLsr, i, j)
                If pxsLsrKEB(idxR) = 255 Then
                    For m = 1 To radius
                        For n = 0 To degStep
                            aa = i - LUTCS(0, n, m)
                            bb = j - LUTCS(1, n, m)
                            If aa < 0 Or aa > wR Or bb < 0 Or bb > hR Then
                                Continue For
                            End If
                            'vote(aa, bb, m) += 1
                            vote(aa, bb) += 1
                        Next
                    Next
                End If
            Next
        Next

        Dim vote2(hLsr * wPxLsr) As Integer
        For j = 0 To hLsr - 1
            For i = 0 To wPxLsr - 1
                vote2((j * wPxLsr) + i) = vote(i, j)
            Next
        Next

        Dim voteTemp As Integer
        For m = 0 To vote2.Length - 2
            For n = m + 1 To vote2.Length - 1
                If vote2(m) > vote2(n) Then
                    voteTemp = vote2(m)
                    vote2(m) = vote2(n)
                    vote2(n) = voteTemp
                End If
            Next
        Next
        '////////////////////////////////////////////////////////////////////////////////////////////
        '///////////////////////// Summing Various Value Custom Window ///////////////////////////////
        Dim sumMask(2, wPxLsr, hLsr) As Integer
        Dim sumAvg(2) As ULong
        Dim sumMax(2, 2) As Integer
        offset = 1
        For j = offset To (hLsr - offset) - 1
            For i = offset To (wPxLsr - offset) - 1
                For m = 0 To 2
                    For n = 0 To 2
                        idxR = DIP.GetIndexR(strLsr, i - offset + n, j - offset + m)
                        idxG = idxR - 1
                        idxB = idxR - 2

                        sumMask(0, i, j) += pxsLsr(idxR)
                        sumMask(1, i, j) += Es(i - offset + n, j - offset + m)
                        'sumMask(2, i, j) += pxsLsrKEB(idxB)
                    Next
                Next

                sumAvg(0) += sumMask(0, i, j)

                ' Red Value
                If sumMax(0, 0) < sumMask(0, i, j) Then
                    sumMax(0, 0) = sumMask(0, i, j)
                    sumMax(0, 1) = i
                    sumMax(0, 2) = j
                End If
                ' Edge
                If sumMax(1, 0) < sumMask(1, i, j) Then
                    sumMax(1, 0) = sumMask(1, i, j)
                    sumMax(1, 1) = j
                    sumMax(1, 2) = j
                End If
                ' Binary
                If sumMax(2, 0) < sumMask(2, i, j) Then
                    sumMax(2, 0) = sumMask(2, i, j)
                    sumMax(2, 1) = iPx
                    sumMax(2, 2) = j
                End If
            Next
        Next

        sumAvg(0) = sumAvg(0) / (hLsr * wPxLsr)
        '////////////////////////////////////////////////////////////////////////////////////////////
        Marshal.Copy(pxsOriR, 0, scOri, pxsOri.Length)
        Marshal.Copy(pxsLsrK, 0, scLsr, pxsLsr.Length)

        bmapOri.UnlockBits(bmdOri)
        bmapLaser.UnlockBits(bmdLaser)
        '////////////////////////////////////////////////////////////////////////////////////////////////////
        gr = Graphics.FromImage(bmapOri)
        gr.DrawRectangle(Pens.Red, xR, yR, wR, hR)

        gr.DrawRectangle(Pens.Red, xR + sumMax(0, 1) - 5, yR + sumMax(0, 2) - 5, 10, 10)
        For aa = 5 To wR - 5 - 1                            ' center x
            For bb = 5 To hR - 5 - 1                        ' center y 
                If vote(aa, bb) > vote2(vote2.Length * 0.1) Then
                    Dim x As Integer = aa + xR
                    Dim y As Integer = bb + yR
                    'gr.DrawRectangle(Pens.Green, x - 5, y - 5, 10, 10)
                    If sumMax(0, 1) = aa And sumMax(0, 2) = bb Then
                        gr.DrawRectangle(Pens.Yellow, x - 5, y - 5, 10, 10)
                        Label1.Text = "Laser X: " + sumMax(0, 1).ToString
                        Label2.Text = "Laser Y: " + sumMax(0, 2).ToString
                    Else
                        'Label1.Text = "Laser X: Not Detect"
                        'Label2.Text = "Laser Y: Not Detect"
                    End If
                End If
            Next
        Next
        '////////////////////////////////////////////////////////////////////////////////////////////////////


        picTCap.Image = bmapOri
        picLsr.Image = bmapLaser
    End Sub

    Private Sub TemplateMatching()
        Timer1.Enabled = False

        '////////////////////////////////////////////////////////////////////////////////////////////////////////
        Dim bmapMatch As New Bitmap(320, 240)
        data = Clipboard.GetDataObject()
        If data.GetDataPresent(GetType(System.Drawing.Bitmap)) Then
            bmapMatch = CType(data.GetData(GetType(System.Drawing.Bitmap)), Image)
        End If

        'If (bmapMatch Is Nothing) Then
        '    Return
        'End If

        'If bmapSmplOri Is Nothing Then
        '    Return
        'End If

        Dim bmdMatch As BitmapData = bmapMatch.LockBits(New Rectangle(0, 0, bmapMatch.Width, bmapMatch.Height),
            ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)
        Dim scMatch As IntPtr = bmdMatch.Scan0
        Dim strMatch As Integer = bmdMatch.Stride
        Dim wMatch As Integer = bmdMatch.Width * 3
        Dim hMatch As Integer = bmdMatch.Height
        Dim btMatch As Integer = Math.Abs(strMatch) * hMatch

        Dim pxsMatch(btMatch - 1) As Byte
        Dim pxsMatchK(btMatch - 1) As Byte
        Dim pxsMatchKE(btMatch - 1) As Byte

        Marshal.Copy(scMatch, pxsMatch, 0, pxsMatch.Length)
        '////////////////////////////////////////////////////////////////////////////////////////////////////////
        Dim bmapSmpl As Bitmap = bmapSmplOri.Clone()
        Dim bmdSmpl As BitmapData = bmapSmpl.LockBits(New Rectangle(0, 0, bmapSmpl.Width, bmapSmpl.Height),
            ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)
        Dim scSmpl As IntPtr = bmdSmpl.Scan0
        Dim strSmpl As Integer = bmdSmpl.Stride
        Dim wSmpl As Integer = bmdSmpl.Width * 3
        Dim hSmpl As Integer = bmdSmpl.Height
        Dim btSmpl As Integer = Math.Abs(strSmpl) * hSmpl

        Dim pxsSmpl(btSmpl - 1) As Byte
        Dim pxsSmplK(btSmpl - 1) As Byte
        Dim pxsSmplKE(btSmpl - 1) As Byte

        Marshal.Copy(scSmpl, pxsSmpl, 0, pxsSmpl.Length)
        '////////////////////////////////////////////////////////////////////////////////////////////////////////
        Dim iPx, idxR, idxG, idxB As Integer
        Dim r, g, b, bl As Integer
        Dim offset As Integer
        '/////////////////////////////////// Match Image Gray Process /////////////////////////////////////
        For j = 0 To hMatch - 1
            For i = 0 To wMatch - 1 Step 3
                iPx = i / 3
                idxR = DIP.GetIndexR(strMatch, iPx, j)
                idxG = idxR - 1
                idxB = idxR - 2

                r = pxsMatch(idxR)
                g = pxsMatch(idxG)
                b = pxsMatch(idxB)
                bl = DIP.GrayValue(r, g, b)

                pxsMatchK(idxR) = bl
                pxsMatchK(idxG) = bl
                pxsMatchK(idxB) = bl
            Next
        Next
        '////////////////////////////////////////////////////////////////////////////////////////////////////
        '/////////////////////////////////// Sample Image Gray Process /////////////////////////////////////
        For j = 0 To hSmpl - 1
            For i = 0 To wSmpl - 1 Step 3
                iPx = i / 3
                idxR = DIP.GetIndexR(strSmpl, iPx, j)
                idxG = idxR - 1
                idxB = idxR - 2

                r = pxsSmpl(idxR)
                g = pxsSmpl(idxG)
                b = pxsSmpl(idxB)
                bl = DIP.GrayValue(r, g, b)

                pxsSmplK(idxR) = bl
                pxsSmplK(idxG) = bl
                pxsSmplK(idxB) = bl
            Next
        Next

        Dim TM(wMatch / 3, hMatch) As Double
        Dim wPxMatch As Integer = wMatch / 3
        Dim wPxSmpl As Integer = wSmpl / 3
        '////////////////////////////////////////////////////////////////////////////////////////////////////
        '////////////////////////////////// Edge Based TM Process /////////////////////////////////////
        Dim tmMax(2) As Integer
        If radEB.Checked Then
            '/////////// Match Edge Process ///////////////////
            Dim ExM(wMatch / 3, hMatch) As Integer
            Dim EyM(wMatch / 3, hMatch) As Integer
            Dim EsM(wMatch / 3, hMatch) As Double
            Dim EoM(wMatch / 3, hMatch) As Double
            offset = 1
            For j = offset To (hMatch - offset) - 1
                For i = offset * 3 To (wMatch - offset * 3) - 1 Step 3
                    iPx = i / 3
                    idxR = DIP.GetIndexR(strMatch, iPx, j)
                    idxG = idxR - 1
                    idxB = idxR - 2

                    For m = 0 To 2
                        For n = 0 To 2
                            Dim temp As Integer = pxsMatchK(DIP.GetIndexR(strMatch, iPx - 1 + n, j - 1 + m))
                            ExM(iPx, j) += temp * DIP.SobelMaskX(n, m)
                            EyM(iPx, j) += temp * DIP.SobelMaskY(n, m)
                        Next
                    Next

                    EsM(iPx, j) = Math.Sqrt(Math.Pow(ExM(iPx, j), 2) + Math.Pow(EyM(iPx, j), 2))
                    Dim temp1 As Integer = DIP.LimitValue(EsM(iPx, j), 0, 255)
                    pxsMatchKE(idxR) = temp1
                    pxsMatchKE(idxG) = temp1
                    pxsMatchKE(idxB) = temp1
                Next
            Next
            '//////////////////// Sample Edge Process /////////////////
            Dim ExS(wSmpl / 3, hSmpl) As Integer
            Dim EyS(wSmpl / 3, hSmpl) As Integer
            Dim EsS(wSmpl / 3, hSmpl) As Double
            Dim EoS(wSmpl / 3, hSmpl) As Double

            offset = 1
            For j = offset To (hSmpl - offset) - 1
                For i = offset * 3 To (wSmpl - offset * 3) - 1 Step 3
                    iPx = i / 3
                    idxR = DIP.GetIndexR(strSmpl, iPx, j)
                    idxG = idxR - 1
                    idxB = idxR - 2

                    For m = 0 To 2
                        For n = 0 To 2
                            Dim temp As Integer = pxsSmplK(DIP.GetIndexR(strSmpl, iPx - 1 + n, j - 1 + m))
                            ExS(iPx, j) += temp * DIP.SobelMaskX(n, m)
                            EyS(iPx, j) += temp * DIP.SobelMaskY(n, m)
                        Next
                    Next

                    EsS(iPx, j) = Math.Sqrt(Math.Pow(ExS(iPx, j), 2) + Math.Pow(EyS(iPx, j), 2))
                    Dim temp1 As Integer = DIP.LimitValue(EsS(iPx, j), 0, 255)
                    pxsSmplKE(idxR) = temp1
                    pxsSmplKE(idxG) = temp1
                    pxsSmplKE(idxB) = temp1
                Next
            Next

            For j = 0 To hMatch - 1
                For i = 0 To wPxMatch - 1
                    For m = 0 To hSmpl - 1 Step 3
                        For n = 0 To wPxSmpl - 1 Step 3
                            Dim iM As Integer = i + n
                            Dim jM As Integer = j + m

                            If (iM > (wPxMatch - 1)) Or (jM > (hMatch - 1)) Then
                                Continue For
                            End If

                            If (ExS(n, m) <> 0 Or EyS(n, m) <> 0) And
                                (ExM(iM, jM) <> 0 Or EyM(iM, jM) <> 0) Then
                                Dim cExSM As Double = ExS(n, m) * ExM(iM, jM)
                                Dim cEySM As Double = EyS(n, m) * EyM(iM, jM)

                                TM(i, j) += (cExSM + cEySM) / (EsS(n, m) * EsM(iM, jM))
                            End If
                        Next
                    Next
                Next
            Next

            For j = 0 To hMatch - hSmpl - 1
                For i = 0 To wPxMatch - wPxSmpl - 1
                    If tmMax(0) < TM(i, j) Then
                        tmMax(0) = TM(i, j)
                        tmMax(1) = i
                        tmMax(2) = j
                    End If
                Next
            Next
        End If
        '///////////////////////////////////////////////////////////////////////////////////////////////////
        '//////////////////////////////////  Gray Based SAD TM Process /////////////////////////////////////
        Dim tmMin(2) As Integer
        Dim MK, SK As Integer
        If radGB.Checked Then
            For j = 0 To hMatch - 1
                For i = 0 To wPxMatch - 1
                    For m = 0 To hSmpl - 1 Step 3
                        For n = 0 To wPxSmpl - 1 Step 3
                            Dim iM As Integer = i + n
                            Dim jM As Integer = j + m

                            If (iM > (wPxMatch - 1)) Or (jM > (hMatch - 1)) Then
                                Continue For
                            End If

                            MK = pxsMatchK(DIP.GetIndexR(strMatch, iM, jM))
                            SK = pxsSmplK(DIP.GetIndexR(strSmpl, n, m))
                            TM(i, j) += Math.Abs(MK - SK)
                        Next
                    Next
                Next
            Next
            tmMin(0) = 100000000
            For j = 0 To hMatch - hSmpl - 1
                For i = 0 To wPxMatch - wPxSmpl - 1
                    If tmMin(0) > TM(i, j) Then
                        tmMin(0) = TM(i, j)
                        tmMin(1) = i
                        tmMin(2) = j
                    End If
                Next
            Next
        End If
        '///////////////////////////////////////////////////////////////////////////////////////////////////

        If radEB.Checked Then
            Marshal.Copy(pxsMatchKE, 0, scMatch, pxsMatchKE.Length)
            Marshal.Copy(pxsSmplKE, 0, scSmpl, pxsSmplKE.Length)
        End If

        If radGB.Checked Then
            Marshal.Copy(pxsMatchK, 0, scMatch, pxsMatchK.Length)
            Marshal.Copy(pxsSmplK, 0, scSmpl, pxsSmplK.Length)
        End If

        bmapMatch.UnlockBits(bmdMatch)
        bmapSmpl.UnlockBits(bmdSmpl)

        gr = Graphics.FromImage(bmapMatch)
        If radEB.Checked Then
            gr.DrawRectangle(Pens.Red, tmMax(1), tmMax(2), wPxSmpl, hSmpl)
            'Label1.Text = tmMax(0)
            'Label2.Text = tmMax(1)
            'Label3.Text = tmMax(2)
        End If

        If radGB.Checked Then
            gr.DrawRectangle(Pens.Red, tmMin(1), tmMin(2), wPxSmpl, hSmpl)
            'Label1.Text = tmMin(0)
            'Label2.Text = tmMin(1)
            'Label3.Text = tmMin(2)
        End If

        picMatch.Image = bmapMatch
        picSample.Image = bmapSmpl

        Timer1.Enabled = True
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        ' Copy image to clipboard
        SendMessage(hHwnd, WM_CAP_EDIT_COPY, 0, 0)

        If cbLsr.Checked Then
            Transform()
        End If

        If cbTM.Checked Then
            TemplateMatching()
        End If
    End Sub

    Private Sub btnSample_Click(sender As System.Object, e As System.EventArgs) Handles btnSample.Click
        If ofd.ShowDialog = Windows.Forms.DialogResult.OK Then
            bmapSmplOri = Image.FromFile(ofd.FileName)
            picSample.Image = bmapSmplOri
        End If
    End Sub

    Private Sub btnMatch_Click(sender As System.Object, e As System.EventArgs)
        TemplateMatching()
    End Sub

    Private Sub btnOb_Click(sender As System.Object, e As System.EventArgs) Handles btnOb.Click
        Obstacle()
    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        LoadDeviceList()
        If lstDevices.Items.Count > 0 Then
            btnStart.Enabled = True
            lstDevices.SelectedIndex = 0
            btnStart.Enabled = True
        Else
            lstDevices.Items.Add("No Capture Device")
            btnStart.Enabled = False
        End If

        btnStop.Enabled = False
        btnSave.Enabled = False
        picPreview.SizeMode = PictureBoxSizeMode.StretchImage

        Dim path As String = Directory.GetCurrentDirectory()
        bmapSmplOri = Image.FromFile(path + "\sample.jpg")
        picSample.Image = bmapSmplOri
    End Sub

    Private Sub Form1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If btnStop.Enabled Then
            ClosePreviewWindow()
        End If
    End Sub

    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        If bmapSmplOri Is Nothing Then
            Return
        End If

        Timer1.Enabled = True

        iDevice = lstDevices.SelectedIndex
        OpenPreviewWindow()
    End Sub

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        Timer1.Enabled = False

        ClosePreviewWindow()
        btnSave.Enabled = False
        btnStart.Enabled = True
        btnStop.Enabled = False
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim data As IDataObject

        ' Copy image to clipboard
        SendMessage(hHwnd, WM_CAP_EDIT_COPY, 0, 0)

        ' Get image from clipboard and convert it to a bitmap
        data = Clipboard.GetDataObject()
        If data.GetDataPresent(GetType(System.Drawing.Bitmap)) Then
            Dim bmap As Bitmap = CType(data.GetData(GetType(System.Drawing.Bitmap)), Image)
            picPreview.Image = bmap
            ClosePreviewWindow()
            btnSave.Enabled = False
            btnStop.Enabled = False
            btnStart.Enabled = True

            If sfdImage.ShowDialog = DialogResult.OK Then
                bmap.Save(sfdImage.FileName, Imaging.ImageFormat.Bmp)
            End If
        End If
    End Sub

    Private Sub btnInfo_Click(sender As System.Object, e As System.EventArgs) Handles btnInfo.Click
        SendMessage(hHwnd, WM_CAP_DLG_VIDEOFORMAT, 0&, 0&)
    End Sub

    Private Sub LoadDeviceList()
        Dim strName As String = Space(100)
        Dim strVer As String = Space(100)
        Dim bReturn As Boolean
        Dim x As Integer = 0

        ' Load name of all avialable devices into the lstDevices
        Do
            ' Get Driver name and version
            bReturn = capGetDriverDescriptionA(x, strName, 100, strVer, 100)

            ' If there was a device add device name to the list
            If bReturn Then lstDevices.Items.Add(strName.Trim)
            x += 1
        Loop Until bReturn = False
    End Sub

    Private Sub OpenPreviewWindow()
        Dim iHeight As Integer = picPreview.Height
        Dim iWidth As Integer = picPreview.Width

        ' Open Preview window in picturebox
        hHwnd = capCreateCaptureWindowA(iDevice, WS_VISIBLE Or WS_CHILD, 0, 0, 320, 240,
                                        picPreview.Handle.ToInt32, 0)

        ' Connect to device
        If SendMessage(hHwnd, WM_CAP_DRIVER_CONNECT, iDevice, 0) Then
            'Set the preview scale
            SendMessage(hHwnd, WM_CAP_SET_SCALE, True, 0)

            'Set the preview rate in milliseconds
            SendMessage(hHwnd, WM_CAP_SET_PREVIEWRATE, 33, 0)

            'Start previewing the image from the camera
            SendMessage(hHwnd, WM_CAP_SET_PREVIEW, True, 0)

            ' Resize window to fit in picturebox
            SetWindowPos(hHwnd, HWND_BOTTOM, 0, 0, picPreview.Width, picPreview.Height, SWP_NOMOVE Or SWP_NOZORDER)

            btnSave.Enabled = True
            btnStop.Enabled = True
            btnStart.Enabled = False
        Else
            ' Error connecting to device close window
            DestroyWindow(hHwnd)

            btnSave.Enabled = False
        End If
    End Sub

    Private Sub ClosePreviewWindow()
        ' Disconnect from device
        SendMessage(hHwnd, WM_CAP_DRIVER_DISCONNECT, iDevice, 0)

        ' close window
        DestroyWindow(hHwnd)
    End Sub
End Class
