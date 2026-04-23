import type { ReactNode } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { FaDiscord, FaGithub, FaTiktok } from "react-icons/fa6";
import InteractiveDotGrid from "./InteractiveDotGrid";
import { externalLinks } from "../content";

type SiteLayoutProps = {
  children: ReactNode;
};

type RouteButtonProps = {
  to: string;
  className?: string;
  active?: boolean;
  children: ReactNode;
};

function RouteButton({
  to,
  className,
  active = false,
  children
}: RouteButtonProps) {
  const navigate = useNavigate();

  return (
    <button
      aria-current={active ? "page" : undefined}
      className={className}
      onClick={() => navigate(to)}
      type="button"
    >
      {children}
    </button>
  );
}

function ExternalButton({
  href,
  className,
  children
}: {
  href: string;
  className?: string;
  children: ReactNode;
}) {
  return (
    <button
      className={className}
      onClick={() => window.open(href, "_blank", "noopener,noreferrer")}
      type="button"
    >
      {children}
    </button>
  );
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
  const isDownload = location.pathname === "/download";
  const isGuide = location.pathname === "/guide";
  const isAbout = location.pathname === "/about";

  return (
    <div className="site-shell">
      <InteractiveDotGrid />

      <header className="site-header">
        <div className="site-header__inner">
          <RouteButton className="site-brand" to="/">
            Uaskus Tweaker
          </RouteButton>

          <nav className="site-nav" aria-label="Primary">
            <RouteButton active={isDownload} className={isDownload ? "is-active" : undefined} to="/download">
              Download
            </RouteButton>
            <RouteButton active={isGuide} className={isGuide ? "is-active" : undefined} to="/guide">
              Guide
            </RouteButton>
            <RouteButton active={isAbout} className={isAbout ? "is-active" : undefined} to="/about">
              About Me
            </RouteButton>
            <div className="site-nav__socials" aria-label="Social links">
              {externalLinks.map((link) => (
                <SocialLink key={link.label} href={link.href} label={link.label} />
              ))}
            </div>
          </nav>
        </div>
      </header>

      <main>
        <div className="page-transition" key={location.pathname}>
          {children}
        </div>
      </main>

      <footer className="site-footer">
        <div className="site-footer__links">
          <RouteButton to="/">Home</RouteButton>
          <RouteButton to="/download">Download</RouteButton>
          <RouteButton to="/guide">Guide</RouteButton>
          <RouteButton to="/about">About Me</RouteButton>
          <ExternalButton
            href="https://github.com/maskus89/uaskus-tweaker"
          >
            GitHub
          </ExternalButton>
        </div>
        <p>&copy; 2026 Uaskus Tweaker. All rights reserved.</p>
      </footer>
    </div>
  );
}
