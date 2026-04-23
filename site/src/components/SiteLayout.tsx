import type { ReactNode } from "react";
import { Link, useLocation } from "react-router-dom";
import { FaDiscord, FaGithub, FaTiktok } from "react-icons/fa6";
import { externalLinks } from "../content";

type SiteLayoutProps = {
  children: ReactNode;
};

function SectionLink({
  label,
  section
}: {
  label: string;
  section: "features" | "download";
}) {
  return <Link to={`/?section=${section}`}>{label}</Link>;
}

function SocialLink({ href, label }: { href: string; label: string }) {
  const icons = {
    GitHub: FaGithub,
    TikTok: FaTiktok,
    Discord: FaDiscord
  };

  const Icon = icons[label as keyof typeof icons];

  return (
    <a
      className="site-nav__social-link"
      href={href}
      target="_blank"
      rel="noreferrer"
      aria-label={label}
      title={label}
    >
      {Icon ? <Icon aria-hidden="true" /> : label}
    </a>
  );
}

export default function SiteLayout({ children }: SiteLayoutProps) {
  const location = useLocation();
  const isGuide = location.pathname === "/guide";
  const isAbout = location.pathname === "/about";

  return (
    <div className="site-shell">
      <div className="site-grid site-grid--primary" />

      <header className="site-header">
        <div className="site-header__inner">
          <Link className="site-brand" to="/">
            Uaskus Tweaker
          </Link>

          <nav className="site-nav" aria-label="Primary">
            <SectionLink label="Features" section="features" />
            <SectionLink label="Download" section="download" />
            <Link className={isGuide ? "is-active" : undefined} to="/guide">
              Guide
            </Link>
            <Link className={isAbout ? "is-active" : undefined} to="/about">
              About Me
            </Link>
            <div className="site-nav__socials" aria-label="Social links">
              {externalLinks.map((link) => (
                <SocialLink key={link.label} href={link.href} label={link.label} />
              ))}
            </div>
          </nav>
        </div>
      </header>

      <main>{children}</main>

      <footer className="site-footer">
        <div className="site-footer__links">
          <Link to="/">Home</Link>
          <Link to="/guide">Guide</Link>
          <Link to="/about">About Me</Link>
          <a
            href="https://github.com/maskus89/uaskus-tweaker"
            target="_blank"
            rel="noreferrer"
          >
            GitHub
          </a>
        </div>
        <p>&copy; 2026 Uaskus Tweaker. All rights reserved.</p>
      </footer>
    </div>
  );
}
