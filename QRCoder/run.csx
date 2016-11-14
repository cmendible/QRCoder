#r "System.Drawing"
#r "QRCoder.dll"
 
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using QRCoder;
 
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    // Read the json request.
    var qrRequest = await req.Content.ReadAsAsync<SimpleVCardRequest>();
 
    // Create the vCard string
    var vCard = "BEGIN:VCARD\n";
    vCard += $"FN:{qrRequest.Name}\n";
    vCard += $"TEL;WORK;VOICE:{qrRequest.Phone}\n";
    vCard += "END:VCARD";
 
    // Generate de QRCode
    QRCodeGenerator qrGenerator = new QRCodeGenerator();
    QRCodeData qrCodeData = qrGenerator.CreateQrCode(vCard, QRCodeGenerator.ECCLevel.Q);
    QRCode qrCode = new QRCode(qrCodeData);
 
    // Save the QRCode as a jpeg image and send it in the response.
    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
    using (MemoryStream ms = new MemoryStream())
    {
        qrCodeImage.Save(ms, ImageFormat.Jpeg);
 
        var response = new HttpResponseMessage()
        {
            Content = new ByteArrayContent(ms.ToArray()),
            StatusCode = HttpStatusCode.OK,
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        return response;
    }    
}
 
// Request class to hold the Name and Phone number used to generated the vCard QR Code
public class SimpleVCardRequest
{
    public string Name { get; set; }
    public string Phone { get; set; }
}