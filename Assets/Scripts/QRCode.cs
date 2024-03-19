using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public static class QRCode
{
    private static Color32[] Encode(string text, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };

        return writer.Write(text);
    }

    public static Texture2D TextToQR(string text)
    {
        Texture2D encodeTexture = new Texture2D(256, 256);
        Color32[] convertPixelToTexture = Encode(text, encodeTexture.width, encodeTexture.height);
        encodeTexture.SetPixels32(convertPixelToTexture);
        encodeTexture.Apply();

        return encodeTexture;
    }
}
