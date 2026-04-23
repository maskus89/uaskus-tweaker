import { aboutParagraphs, externalLinks } from "../content";

export default function AboutPage() {
  return (
    <section className="page page--about">
      <div className="page__hero">
        <p className="eyebrow">About</p>
        <h1>About Me</h1>
        <p>Creator of Uaskus Tweaker.</p>
      </div>

      <div className="stack">
        <article className="content-card">
          <h2>Who Am I?</h2>
          <p className="content-card__eyebrow">Creator profile</p>
          {aboutParagraphs.map((paragraph) => (
            <p key={paragraph}>{paragraph}</p>
          ))}
        </article>

        <article className="content-card">
          <h2>Get In Touch</h2>
          <p className="content-card__eyebrow">Communication channels</p>
          <p>
            I would love to hear from you. Whether you have questions, feedback,
            or just want to say hi, feel free to reach out through the platforms
            below.
          </p>

          <div className="social-row">
            {externalLinks
              .filter((link) => link.label !== "GitHub")
              .map((link) => (
                <a
                  className="social-chip"
                  href={link.href}
                  key={link.label}
                  target="_blank"
                  rel="noreferrer"
                >
                  {link.label}
                </a>
              ))}
          </div>
        </article>
      </div>
    </section>
  );
}
