import { useLayoutEffect } from "react";
import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import SiteLayout from "./components/SiteLayout";
import AboutPage from "./pages/AboutPage";
import DownloadPage from "./pages/DownloadPage";
import GuidePage from "./pages/GuidePage";
import HomePage from "./pages/HomePage";

function ScrollToTop() {
  const location = useLocation();

  useLayoutEffect(() => {
    const scrollRoot = document.scrollingElement ?? document.documentElement;

    const resetScroll = () => {
      window.scrollTo(0, 0);
      scrollRoot.scrollTop = 0;
      document.body.scrollTop = 0;
    };

    resetScroll();

    const frameId = window.requestAnimationFrame(resetScroll);
    const timeoutId = window.setTimeout(resetScroll, 0);

    return () => {
      window.cancelAnimationFrame(frameId);
      window.clearTimeout(timeoutId);
    };
  }, [location.key]);

  return null;
}

export default function App() {
  return (
    <SiteLayout>
      <ScrollToTop />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/download" element={<DownloadPage />} />
        <Route path="/guide" element={<GuidePage />} />
        <Route path="/about" element={<AboutPage />} />
        <Route path="*" element={<Navigate replace to="/" />} />
      </Routes>
    </SiteLayout>
  );
}
