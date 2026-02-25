import { Outlet, ScrollRestoration } from "react-router-dom";
import NavBar from "./NavBar";
import MobileNavBar from "./MobileNavBar";
import BurgerMenu from "./BurgerMenu";
import HelpFeedbackWidget from "@/features/help-feedback/components/HelpFeedbackWidget";

function AppLayout() {
  return (
    <div className="min-h-screen bg-linear-to-br from-[#e9eced] to-[#bec5ca]">
      <BurgerMenu />
      <NavBar />
      <MobileNavBar />
      <div className="p-6 sm:pt-15 md:pt-0 lg:p-8 lg:px-12 lg:pt-6 lg:pb-0">
        <Outlet />
        <ScrollRestoration />
      </div>
      <HelpFeedbackWidget />
    </div>
  );
}

export default AppLayout;
