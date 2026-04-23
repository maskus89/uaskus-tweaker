import { Navigate, Route, Routes } from "react-router-dom";
import SiteLayout from "./components/SiteLayout";
import AboutPage from "./pages/AboutPage";
import GuidePage from "./pages/GuidePage";
import HomePage from "./pages/HomePage";

export default function App() {
  return (
    <SiteLayout>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/guide" element={<GuidePage />} />
        <Route path="/about" element={<AboutPage />} />
        <Route path="*" element={<Navigate replace to="/" />} />
      </Routes>
    </SiteLayout>
  );
}
