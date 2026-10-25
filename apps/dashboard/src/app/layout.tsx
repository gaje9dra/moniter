import type { Metadata } from "next";
import "./styles.css";
export const metadata: Metadata = { title: "Moniter | Endpoint Administration", description: "Authorized endpoint management foundation." };
export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) { return <html lang="en"><body>{children}</body></html>; }
