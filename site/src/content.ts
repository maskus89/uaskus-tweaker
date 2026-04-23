export type ExternalLink = {
  label: string;
  href: string;
};

export type FeatureCard = {
  title: string;
  description: string;
  eyebrow: string;
};

export type GuideSection = {
  step: string;
  title: string;
  description: string;
  bullets: string[];
  note?: {
    title: string;
    description: string;
  };
};

export const externalLinks: ExternalLink[] = [
  { label: "GitHub", href: "https://github.com/maskus89/uaskus-tweaker" },
  { label: "TikTok", href: "https://www.tiktok.com/@uaskustweaker" },
  { label: "Discord", href: "https://discord.gg/GRCdvgtX" }
];

export const featureCards: FeatureCard[] = [
  {
    title: "Performance Boost",
    description:
      "Optimize CPU, memory, and disk usage for faster system responsiveness and improved gaming performance.",
    eyebrow: "Performance"
  },
  {
    title: "Kill Telemetry",
    description:
      "Shut down Microsoft's data collection, disable tracking services, and stop Windows from phoning home. Your PC, your privacy.",
    eyebrow: "Privacy"
  },
  {
    title: "Deep Debloating",
    description:
      "Surgically remove Candy Crush, Xbox apps, Cortana, and dozens of other pre-installed Windows bloatware with one click.",
    eyebrow: "Cleanup"
  },
  {
    title: "Gaming Optimizations",
    description:
      "Apply tweaks specifically designed to reduce input lag and improve frame rates in games.",
    eyebrow: "Gaming"
  },
  {
    title: "System Tweaks",
    description:
      "Access hidden Windows settings and apply proven optimizations with just one click.",
    eyebrow: "Control"
  },
  {
    title: "Backup & Restore",
    description:
      "Create system restore points before applying changes so you can always revert if needed.",
    eyebrow: "Safety"
  }
];

export const requirements = [
  "Windows 10 or Windows 11 (64-bit)",
  "Administrator privileges",
  "No additional runtime required (self-contained .exe)",
  "50 MB free disk space"
] as const;

export const previewItems = [
  "Performance Boost",
  "Kill Telemetry",
  "Deep Debloating",
  "Gaming Optimizations"
] as const;

export const guideSections: GuideSection[] = [
  {
    step: "1",
    title: "Run as Administrator",
    description:
      "Uaskus Tweaker requires administrator privileges to modify system settings and apply optimizations.",
    bullets: [
      "Right-click on UaskusTweaks.exe",
      "Select \"Run as administrator\"",
      "Click Yes when prompted by User Account Control (UAC)"
    ],
    note: {
      title: "Important",
      description:
        "The program will not function correctly without administrator privileges. Always run as admin."
    }
  },
  {
    step: "2",
    title: "Navigate the GUI",
    description:
      "Use the sidebar to switch between tweak categories and review each group before applying changes.",
    bullets: [
      "Essential Tweaks for recommended everyday changes",
      "Performance Tweaks for system speed improvements",
      "Privacy Tweaks to reduce telemetry and data collection",
      "Network Tweaks for connection-related optimizations",
      "Debloat Windows to remove unnecessary built-in apps",
      "Gaming Tweaks for latency and frame rate focused changes"
    ]
  },
  {
    step: "3",
    title: "Apply Tweaks",
    description:
      "Each tweak includes a name, description, and risk indicator. Review them carefully before applying.",
    bullets: [
      "Read the description of each tweak before enabling it",
      "Some tweaks require a system restart to take effect",
      "Use Select All to enable all tweaks in a category",
      "Applied tweaks are shown in the built-in log panel"
    ]
  },
  {
    step: "4",
    title: "Create a Restore Point",
    description:
      "Creating a restore point before changing Windows settings gives you a safety net if you want to roll changes back later.",
    bullets: [
      "Allow the app to create a restore point before making changes",
      "The process only takes a short moment",
      "You can use Windows System Restore if something goes wrong"
    ],
    note: {
      title: "Strongly Recommended",
      description:
        "Always create a restore point before applying high-risk tweaks or large preset bundles."
    }
  }
];

export const proTips = [
  "For Gaming: apply the Gaming Optimizations set first.",
  "For Privacy: start with telemetry and tracking-related tweaks.",
  "For Speed: combine performance tweaks with debloating.",
  "Always create a restore point before large changes."
] as const;

export const aboutParagraphs = [
  "Hello there. My name is maskus and I am the creator of Uaskus Tweaker. I care deeply about Windows performance, user privacy, and getting maximum responsiveness out of a PC.",
  "I am a young developer from Norway who loves coding and building tools that make a practical difference. Competitive gaming shaped a lot of this project, because a fast and consistent system matters.",
  "Uaskus Tweaker was created to help people remove unnecessary Windows friction, reclaim performance, and get a cleaner experience from their machines."
] as const;
