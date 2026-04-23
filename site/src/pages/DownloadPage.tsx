import { requirements } from "../content";

const downloadItems = [
  "Portable package with the modern WPF interface",
  "Debloating, privacy, performance, and gaming-focused tweak presets",
  "Restore point creation before applying system changes",
  "Clear categories, risk labels, and built-in execution logging"
] as const;

const setupNotes = [
  "Extract the ZIP archive to any folder on your PC.",
  "Launch UaskusTweaks.exe as Administrator.",
  "Review tweaks carefully before applying them on your main machine.",
  "Restart Windows after major tweak groups when prompted."
] as const;

export default function DownloadPage() {
  return (
    <section className="page page--download">
      <div className="page__hero">
        <p className="eyebrow">Release</p>
        <h1>Download Uaskus Tweaker</h1>
        <p>
          Grab the latest build, review the requirements, and get your Windows
          optimization setup running in a few minutes.
        </p>
      </div>

      <div className="download-grid">
        <div className="download-card">
          <p className="eyebrow">v2.0.0 - WPF Edition</p>
          <h2>Ready for Windows 10 and 11</h2>
          <p>
            Free, open source Windows optimizer with a modern GUI. Download the
            latest portable release, extract it, and run it with administrator
            privileges.
          </p>
          <a
            className="button button--primary"
            href="https://github.com/maskus89/uaskus-tweaker/releases/latest/download/UaskusTweaks.zip"
          >
            Download ZIP
          </a>

          <ul className="bullet-list">
            {downloadItems.map((item) => (
              <li key={item}>{item}</li>
            ))}
          </ul>
        </div>

        <div className="requirements-card">
          <div className="stack">
            <div>
              <p className="content-card__eyebrow">Requirements</p>
              <h3>Before You Download</h3>
              <ul>
                {requirements.map((item) => (
                  <li key={item}>{item}</li>
                ))}
              </ul>
            </div>

            <div>
              <p className="content-card__eyebrow">Quick Setup</p>
              <ul className="bullet-list">
                {setupNotes.map((item) => (
                  <li key={item}>{item}</li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
