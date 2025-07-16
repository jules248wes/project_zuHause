using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using zuHause.DTOs;
using zuHause.Interfaces;

namespace zuHause.Services
{
    /// <summary>
    /// 基於 SixLabors.ImageSharp 的圖片處理器實作
    /// 提供 JPG/PNG 轉 WebP 和縮圖生成功能
    /// </summary>
    public class ImageSharpProcessor : IImageProcessor
    {
        /// <summary>
        /// 將圖片轉換為 WebP 格式
        /// </summary>
        /// <param name="sourceStream">來源圖片串流</param>
        /// <param name="maxWidth">最大寬度 (可選，null 表示不限制)</param>
        /// <param name="quality">品質 (1-100，預設 80)</param>
        /// <returns>處理結果，包含轉換後的圖片串流和元數據</returns>
        public async Task<ImageProcessingResult> ConvertToWebPAsync(
            Stream sourceStream, 
            int? maxWidth = null, 
            int quality = 80)
        {
            try
            {
                // 重置串流位置
                sourceStream.Position = 0;

                // 載入圖片並偵測原始格式
                using var image = await Image.LoadAsync(sourceStream);
                var originalFormat = image.Metadata.DecodedImageFormat?.Name ?? "Unknown";

                // 如果指定了最大寬度，進行等比例縮放
                if (maxWidth.HasValue && image.Width > maxWidth.Value)
                {
                    var ratio = (double)maxWidth.Value / image.Width;
                    var newHeight = (int)(image.Height * ratio);
                    image.Mutate(x => x.Resize(maxWidth.Value, newHeight));
                }

                // 建立輸出串流
                var outputStream = new MemoryStream();

                // 設定 WebP 編碼器選項
                var webpEncoder = new WebpEncoder
                {
                    Quality = quality,
                    Method = WebpEncodingMethod.BestQuality
                };

                // 轉換為 WebP 格式
                await image.SaveAsync(outputStream, webpEncoder);

                // 重置輸出串流位置以供讀取
                outputStream.Position = 0;

                // 建立成功的處理結果
                return ImageProcessingResult.CreateSuccess(
                    outputStream,
                    image.Width,
                    image.Height,
                    originalFormat);
            }
            catch (UnknownImageFormatException)
            {
                return ImageProcessingResult.CreateFailure("不支援的圖片格式");
            }
            catch (InvalidImageContentException)
            {
                return ImageProcessingResult.CreateFailure("無效的圖片內容");
            }
            catch (Exception ex)
            {
                return ImageProcessingResult.CreateFailure($"圖片處理失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 生成指定尺寸的縮圖
        /// </summary>
        /// <param name="sourceStream">來源圖片串流</param>
        /// <param name="width">目標寬度</param>
        /// <param name="height">目標高度</param>
        /// <returns>處理結果，包含縮圖串流和元數據</returns>
        public async Task<ImageProcessingResult> GenerateThumbnailAsync(
            Stream sourceStream, 
            int width, 
            int height)
        {
            try
            {
                // 驗證輸入參數
                if (width <= 0 || height <= 0)
                {
                    return ImageProcessingResult.CreateFailure("縮圖尺寸必須大於 0");
                }

                // 重置串流位置
                sourceStream.Position = 0;

                // 載入圖片並偵測原始格式
                using var image = await Image.LoadAsync(sourceStream);
                var originalFormat = image.Metadata.DecodedImageFormat?.Name ?? "Unknown";

                // 產生縮圖 - 使用固定尺寸裁切模式
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center
                }));

                // 建立輸出串流
                var outputStream = new MemoryStream();

                // 設定 WebP 編碼器選項 (縮圖使用較高品質)
                var webpEncoder = new WebpEncoder
                {
                    Quality = 85,
                    Method = WebpEncodingMethod.BestQuality
                };

                // 轉換為 WebP 格式
                await image.SaveAsync(outputStream, webpEncoder);

                // 重置輸出串流位置以供讀取
                outputStream.Position = 0;

                // 建立成功的處理結果
                return ImageProcessingResult.CreateSuccess(
                    outputStream,
                    width,
                    height,
                    originalFormat);
            }
            catch (UnknownImageFormatException)
            {
                return ImageProcessingResult.CreateFailure("不支援的圖片格式");
            }
            catch (InvalidImageContentException)
            {
                return ImageProcessingResult.CreateFailure("無效的圖片內容");
            }
            catch (Exception ex)
            {
                return ImageProcessingResult.CreateFailure($"縮圖生成失敗: {ex.Message}");
            }
        }
    }
}