import { IoCart, IoHeart } from "react-icons/io5";

import { PATHS } from "@/routes/paths";

import NavBarButton from "./NavBarButton";
import ProfileButton from "./ProfileButton";

function NavBarActions() {
  return (
    <div className="flex items-center gap-2">
      <NavBarButton to={PATHS.cart} bg="light" icon={<IoCart />} />
      <NavBarButton
        to={PATHS.wishlist}
        className="!text-red-500"
        bg="light"
        icon={<IoHeart />}
      />
      <ProfileButton />
    </div>
  );
}

export default NavBarActions;
