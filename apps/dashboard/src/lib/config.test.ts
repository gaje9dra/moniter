import { describe, expect, it } from "vitest";
import { getApiBaseUrl } from "./config";

describe("getApiBaseUrl", () => {
  it("uses the local development default", () => {
    expect(getApiBaseUrl("")).toBe("http://localhost:5080");
  });

  it("normalizes an absolute HTTP URL", () => {
    expect(getApiBaseUrl(" http://127.0.0.1:5080/ ")).toBe("http://127.0.0.1:5080");
  });

  it("rejects relative URLs", () => {
    expect(() => getApiBaseUrl("/api")).toThrow(/absolute HTTP\(S\) URL/);
  });

  it("rejects non-HTTP protocols", () => {
    expect(() => getApiBaseUrl("file:///tmp/api")).toThrow(/HTTP or HTTPS/);
  });
});
