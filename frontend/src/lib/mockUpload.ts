/** Simulates Cloudinary / image upload for offline SPA demo. */
export function mockUploadImage(): Promise<{ secure_url: string }> {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        secure_url:
          "https://images.pexels.com/photos/2888150/pexels-photo-2888150.jpeg?auto=compress&cs=tinysrgb&w=1200",
      });
    }, 500);
  });
}
