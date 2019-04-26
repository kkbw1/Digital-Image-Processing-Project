Imports System.Drawing.Image
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class Lock
    Public Shared Function Inv(ByVal bmp As Bitmap) As Bitmap
        Dim bmd As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
        ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        Dim scan0 As IntPtr = bmd.Scan0
        Dim stride As Integer = bmd.Stride

        Dim bytes As Integer = Math.Abs(bmd.Stride) * bmp.Height
        Dim pixels(bytes - 1) As Byte
        Marshal.Copy(scan0, pixels, 0, pixels.Length)

        For i As Integer = 0 To pixels.Length - 1
            pixels(i) = 255 - pixels(i)
        Next

        Marshal.Copy(pixels, 0, scan0, pixels.Length)
        bmp.UnlockBits(bmd)

        Return bmp
    End Function

    Public Shared Function Gray(ByVal bmp As Bitmap) As Bitmap
        Dim r, g, b As Integer
        Dim k As Integer

        Dim bmd As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        Dim scan0 As IntPtr = bmd.Scan0
        Dim stride As Integer = bmd.Stride
        Dim bytes As Integer = Math.Abs(bmd.Stride) * bmp.Height
        Dim pixels(bytes - 1) As Byte

        Marshal.Copy(scan0, pixels, 0, pixels.Length)

        For i As Integer = 0 To pixels.Length - 1 Step 3
            b = pixels(i)
            g = pixels(i + 1)
            r = pixels(i + 2)
            k = (r + g + b) / 3

            pixels(i) = k
            pixels(i + 1) = k
            pixels(i + 2) = k
        Next

        Marshal.Copy(pixels, 0, scan0, pixels.Length)
        bmp.UnlockBits(bmd)

        Return bmp
    End Function

    Public Shared Function TP(ByVal bmapOri As Bitmap) As Bitmap
        Dim r, g, b As Integer
        Dim k As Integer

        Dim bmdOri As BitmapData = bmapOri.LockBits(New Rectangle(0, 0, bmapOri.Width, bmapOri.Height),
ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        Dim scan0Ori As IntPtr = bmdOri.Scan0
        Dim strideOri As Integer = bmdOri.Stride
        Dim widthOri As Integer = bmdOri.Width * 3
        Dim heightOri As Integer = bmdOri.Height

        Dim bytesOri As Integer = Math.Abs(strideOri) * heightOri
        Dim pxsOri(bytesOri - 1) As Byte
        Dim pxsGray(bytesOri - 1) As Byte
        Dim pxsEdge(bytesOri - 1) As Byte

        Marshal.Copy(scan0Ori, pxsOri, 0, pxsOri.Length)

        Dim bmapTrans As New Bitmap(bmapOri.Width, bmapOri.Height)
        Dim bmdTrans As BitmapData = bmapTrans.LockBits(New Rectangle(0, 0, bmapTrans.Width, bmapTrans.Height),
ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        Dim scan0Trans As IntPtr = bmdTrans.Scan0
        Dim strideTrans As Integer = bmdTrans.Stride
        Dim widthTrans As Integer = bmdTrans.Width * 3
        Dim heightTrans As Integer = bmdTrans.Height

        Dim bytesTrans As Integer = Math.Abs(strideTrans) * heightTrans
        Dim pxsTrans(bytesTrans - 1) As Byte

        Dim n0 As Integer
        Dim h0 As Integer

        Dim n1 As Integer
        Dim h1 As Integer

        ' Grayscale
        For j = 0 To heightOri - 1
            h0 = j * strideOri
            For i = 0 To widthOri - 1 Step 3
                n0 = i + h0

                r = pxsOri(n0)
                g = pxsOri(n0 + 1)
                b = pxsOri(n0 + 2)
                k = (r + g + b) / 3

                pxsGray(n0) = k
                pxsGray(n0 + 1) = k
                pxsGray(n0 + 2) = k
            Next
        Next

        ' Edge
        Dim bl(8) As Integer
        Dim diff As Integer
        For j = 1 To heightOri - 2
            h0 = j * strideOri
            For i = 3 To (widthOri - 3) - 1 Step 3
                n0 = i + h0

                bl(0) = pxsGray((n0 - strideOri) - 1)
                bl(1) = pxsGray(n0 - strideOri)
                bl(2) = pxsGray((n0 - strideOri) + 1)

                bl(3) = pxsGray(n0 - 1)
                bl(5) = pxsGray(n0 + 1)

                bl(6) = pxsGray((n0 + strideOri) - 1)
                bl(7) = pxsGray(n0 + strideOri)
                bl(8) = pxsGray((n0 + strideOri) + 1)

                diff = 0
                bl(4) = 0
                For m = 0 To 3
                    diff = Math.Abs(bl(m) - bl(8 - m))
                    If bl(4) < diff Then
                        bl(4) = diff
                    End If
                Next

                pxsEdge(n0) = bl(4)
                pxsEdge(n0 + 1) = bl(4)
                pxsEdge(n0 + 2) = bl(4)
            Next
        Next

        ' Copy To bmapTrans From Processed Image
        For j = 0 To heightOri - 1
            h0 = j * strideOri
            h1 = j * strideTrans
            For i = 0 To widthOri - 1 Step 3
                n0 = i + h0
                n1 = i + h1

                pxsTrans(n1) = pxsEdge(n0)
                pxsTrans(n1 + 1) = pxsEdge(n0 + 1)
                pxsTrans(n1 + 2) = pxsEdge(n0 + 2)

                'pxsTrans(n1) = pxsGray(n0)
                'pxsTrans(n1 + 1) = pxsGray(n0 + 1)
                'pxsTrans(n1 + 2) = pxsGray(n0 + 2)
            Next
        Next

        Marshal.Copy(pxsTrans, 0, scan0Trans, pxsTrans.Length)

        bmapOri.UnlockBits(bmdOri)
        bmapTrans.UnlockBits(bmdTrans)

        Return bmapTrans
    End Function
End Class
