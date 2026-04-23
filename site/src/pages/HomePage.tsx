import { useEffect, useRef } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { featureCards, previewItems, requirements } from "../content";

export default function HomePage() {
  const [searchParams] = useSearchParams();
  const featuresRef = useRef<HTMLElement | null>(null);
  const downloadRef = useRef<HTMLElement | null>(null);

  useEffect(() => {
    const section = searchParams.get("section");
    const targets = {
      features: featuresRef.current,
      download: downloadRef.current
    };

    if (section === "features" || section === "download") {
      targets[section]?.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  }, [searchParams]);

  return (
    <>
      <section className="hero">
        <div className="hero__grid">
          <div className="hero__copy">
            <h1>The Ultimate Gaming Performance Utility</h1>
            <p className="hero__tagline">Guaranteed Performance Increase</p>
            <p className="hero__description">
              Uaskus Tweaker strips away Windows bloatware, disables telemetry,
              and optimizes your system for peak performance. Now featuring a
              modern WPF graphical interface, take back control of your PC with
              one powerful tool.
            </p>

            <div className="hero__ticker" aria-label="Core capabilities">
              {previewItems.map((item) => (
                <span className="hero__ticker-chip" key={item}>
                  {item}
                </span>
              ))}
            </div>

            <div className="hero__actions">
              <a
                className="button button--primary"
                href="https://github.com/maskus89/uaskus-tweaker/releases/latest/download/UaskusTweaks.zip"
              >
                Download Now
              </a>
              <Link className="button button--secondary" to="/?section=features">
                Learn More
              </Link>
            </div>
          </div>
        </div>
      </section>

      <section className="section" id="features" ref={featuresRef}>
        <div className="section__heading">
          <p className="eyebrow">Features</p>
          <h2>Powerful Features</h2>
          <p>
            Everything you need to optimize your Windows experience without
            digging through scripts and registry guides.
          </p>
        </div>

        <div className="feature-grid">
          {featureCards.map((card, index) => (
            <article className="feature-card" key={card.title}>
              <span className="feature-card__index">
                {String(index + 1).padStart(2, "0")}
              </span>
              <p className="feature-card__eyebrow">{card.eyebrow}</p>
              <h3>{card.title}</h3>
              <p>{card.description}</p>
            </article>
          ))}
        </div>
      </section>

      <section className="section section--compact">
        <div className="quote-card">
          <p className="quote-card__text">
            This is exactly what Windows needs. Clean, effective, and doesn't
            break your system. Highly recommended for anyone tired of
            Microsoft's bloatware.
          </p>
          <div className="quote-card__author">
            <div className="quote-card__avatar">CT</div>
            <div>
              <strong>Chris Titus</strong>
              <span>Tech YouTuber & Linux Advocate</span>
            </div>
          </div>
        </div>
      </section>

      <section className="section section--download" id="download" ref={downloadRef}>
        <div className="download-grid">
          <div className="download-card">
            <p className="eyebrow">v2.0.0 - WPF Edition</p>
            <h2>Download Uaskus Tweaker</h2>
            <p>
              Free, open source Windows optimizer with a modern GUI. No
              installation required, just extract and run.
            </p>
            <a
              className="button button--primary"
              href="https://github.com/maskus89/uaskus-tweaker/releases/latest/download/UaskusTweaks.zip"
            >
              Download for Windows
            </a>
          </div>

          <div className="requirements-card">
            <h3>System Requirements</h3>
            <ul>
              {requirements.map((item) => (
                <li key={item}>{item}</li>
              ))}
            </ul>
          </div>
        </div>
      </section>
    </>
  );
}
