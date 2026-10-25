const DEFAULT_API_BASE_URL = "http://localhost:5080";
export function getApiBaseUrl(value = process.env.MONITER_API_BASE_URL): string {
 const candidate = value?.trim() || DEFAULT_API_BASE_URL; let parsed: URL;
 try { parsed = new URL(candidate); } catch { throw new Error("MONITER_API_BASE_URL must be an absolute HTTP(S) URL."); }
 if (!["http:", "https:"].includes(parsed.protocol)) throw new Error("MONITER_API_BASE_URL must use HTTP or HTTPS.");
 if (process.env.NODE_ENV === "production" && parsed.protocol !== "https:") throw new Error("MONITER_API_BASE_URL must use HTTPS in production.");
 return parsed.toString().replace(/\/$/, "");
}
export { DEFAULT_API_BASE_URL };
