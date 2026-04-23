import { guideSections, proTips } from "../content";

export default function GuidePage() {
  return (
    <section className="page page--guide">
      <div className="page__hero">
        <p className="eyebrow">Documentation</p>
        <h1>User Guide</h1>
        <p>
          Learn how to use Uaskus Tweaker v2.0.0 WPF Edition to optimize your
          Windows system.
        </p>
      </div>

      <div className="stack">
        {guideSections.map((section) => (
          <article className="content-card" key={section.step}>
            <h2>
              <span className="step-pill">{section.step}</span>
              {section.title}
            </h2>
            <p className="content-card__eyebrow">Operational sequence</p>
            <p>{section.description}</p>
            <ul className="bullet-list">
              {section.bullets.map((bullet) => (
                <li key={bullet}>{bullet}</li>
              ))}
            </ul>

            {section.note ? (
              <div className="note-box">
                <strong>{section.note.title}</strong>
                <p>{section.note.description}</p>
              </div>
            ) : null}
          </article>
        ))}

        <article className="content-card">
          <h2>Pro Tips</h2>
          <p className="content-card__eyebrow">Optimization notes</p>
          <ul className="bullet-list">
            {proTips.map((tip) => (
              <li key={tip}>{tip}</li>
            ))}
          </ul>
        </article>
      </div>
    </section>
  );
}
