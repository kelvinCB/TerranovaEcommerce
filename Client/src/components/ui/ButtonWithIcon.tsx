import { cloneElement, ReactElement } from "react";
import { Link, LinkProps } from "react-router-dom";

interface ButtonWithIconProps extends Omit<LinkProps, "to"> {
  icon: ReactElement<{ size?: number; className?: string }>;
  size?: number;
  to: LinkProps["to"];
}

function ButtonWithIcon({
  icon,
  size = 50,
  className,
  to,
  children,
  ...props
}: ButtonWithIconProps) {
  return (
    <Link
      to={to}
      className={
        (className ? className + " " : "") +
        "bg-brand-500 hover:bg-brand-700 hover:text-brand-100 text-brand-50 flex cursor-pointer items-center gap-3 rounded-[22rem] py-1 pr-1 pl-6 text-xl font-medium"
      }
      {...props}
    >
      {children}
      {cloneElement(icon, { size })}
    </Link>
  );
}

export default ButtonWithIcon;
