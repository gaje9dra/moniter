import { getApiBaseUrl } from "@/lib/config";
type HealthResponse = { status?: string; service?: string };
async function readBackendHealth(): Promise<{ ok: boolean; label: string; detail: string }> {
 let baseUrl: string;
 try { baseUrl = getApiBaseUrl(); } catch { return { ok: false, label: "Configuration error", detail: "Review MONITER_API_BASE_URL." }; }
 try {
  const response = await fetch(`${baseUrl}/health/live`, { cache: "no-store", signal: AbortSignal.timeout(2500) });
  if (!response.ok) return { ok: false, label: "Unavailable", detail: `API returned HTTP ${response.status}.` };
  const body = (await response.json()) as HealthResponse;
  return { ok: body.status === "ok", label: body.status === "ok" ? "Connected" : "Unexpected response", detail: body.service ? `Live API response from ${body.service}.` : "Live API response received." };
 } catch { return { ok: false, label: "Unavailable", detail: "The API did not respond. Start the backend and reload." }; }
}
export default async function HomePage() {
 const health = await readBackendHealth();
 return <main className="page"><aside className="sidebar"><div className="brand-mark" aria-hidden="true">M</div><div className="brand-name">MONITER<span>FOUNDATION</span></div>
 <nav aria-label="Main navigation"><a className="nav-link active" href="#overview" aria-current="page">Overview</a><span className="nav-link disabled">Devices <small>Later phase</small></span><span className="nav-link disabled">Activity <small>Later phase</small></span><span className="nav-link disabled">Policies <small>Later phase</small></span></nav><div className="sidebar-foot">Authorized company-managed endpoints only.</div></aside>
 <section className="content" id="overview"><header className="topbar"><div><p className="eyebrow">ADMINISTRATION / FOUNDATION</p><h1>System overview</h1></div><span className="env-pill">Development</span></header>
 <section className="intro"><p className="eyebrow">FOUNDATION RELEASE</p><h2>Operational clarity starts here.</h2><p>This shell verifies the actual backend connection. Monitoring features are not enabled in this foundation.</p></section>
 <section className="panel" aria-labelledby="health-heading"><div className="panel-header"><div><p className="eyebrow">SERVICE STATUS</p><h2 id="health-heading">Backend connectivity</h2></div><span className={health.ok ? "status status-ok" : "status status-error"}><span aria-hidden="true" className="status-dot" />{health.label}</span></div><p className="health-detail">{health.detail}</p><p className="endpoint-label">Endpoint</p><code>/health/live</code></section>
 <section className="notice" role="note"><strong>Foundation only</strong><p>No employee activity, browsing history, messages, form contents, credentials, or screen content are collected by this application.</p></section><footer>MONITER · FOUNDATION · VERSION 0.1.0</footer></section></main>;
}
