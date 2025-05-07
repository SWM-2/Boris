namespace BorisProxy
{
    class StreamUtils
    {
        public static async Task PipeStream(Stream input, Stream output, TcpSource src, bool dir)
        {
            byte[] buffer = new byte[4096];
            try
            {
                int bytesRead;
                while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    if(dir)
                    {
                        src.Upload(bytesRead);
                    }else{
                        src.Download(bytesRead);
                    }
                    await output.WriteAsync(buffer, 0, bytesRead);
                    await output.FlushAsync();
                }
            }
            catch (IOException)
            {
                Console.WriteLine($" disconnected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" error: {ex.Message}");
            }
        }
    }
}