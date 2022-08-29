// NOTE: .NET 6 is required
// See: https://www.meziantou.net/generating-and-downloading-a-file-in-a-blazor-webassembly-application.htm#blazor-webassembly-o-494245
export class ObjectUrlStorage {

  static Create = (fileName: string, contentType: string, byteArray : BlobPart): string => {
    const fileBits = [ byteArray ];
    const options : FilePropertyBag = {
      type: contentType
    };
    const file : File = new File(fileBits, fileName, options);
    return URL.createObjectURL(file);
  };

  static GetAsString = (url: string): string => {
    const text = (async (): Promise<string> => {
      const init : RequestInit = { }
      const response: Response = await this.fetchWithTimeout(url, init);
      if (!response.ok) {
        const message = `Failed to fetch. Status ${response.status}`;
        console.error(message, response)
        throw new Error(message);
      }
      const text = await response.text()
      console.log("bbbb", text);
      return text;
    })() as unknown as string;
    console.log("aaaa", text)
    return text;
  };

  // https://dmitripavlutin.com/timeout-fetch-request/#2-timeout-a-fetch-request
  static fetchWithTimeout = async(url : string, init : RequestInit, milliseconds : number = 1000) => {
    const controller = new AbortController();
    const id = setTimeout(() => {
      controller.abort();
      console.log("Fetch was aborted.")
    }, milliseconds);
    const response = await fetch(url, {
      ...init,
      signal: controller.signal
    });
    clearTimeout(id);
    return response;
  }

}
