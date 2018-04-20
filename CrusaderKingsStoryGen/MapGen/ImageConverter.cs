// <copyright file="ImageConverter.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

/*
 * Created by Rajan Tawate.
 * User: Owner
 * Date: 9/3/2006
 * Time: 8:00 PM
 *

 */

using System.Drawing;
using System.IO;

/// <summary>
/// Description of ImageConverter.
/// </summary>
public static class ImageConverter
	{
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
		{
			MemoryStream ms = new MemoryStream();
			imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Bmp);
			return  ms.ToArray();
		}

        public static Image byteArrayToImage(byte[] byteArrayIn)
		{
			MemoryStream ms = new MemoryStream(byteArrayIn);
			Image returnImage = Image.FromStream(ms);
			return returnImage;
		}
	}

