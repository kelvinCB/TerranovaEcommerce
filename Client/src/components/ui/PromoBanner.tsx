import { IoArrowForwardCircle } from "react-icons/io5";
import ButtonWithIcon from "./ButtonWithIcon";
import Heading from "./Heading";
import Paragraph from "./Paragraph";
import { ReactNode } from "react";
import { cn } from "@/lib/utils";

interface PromoBannerProps {
  linkTo: string;
  glassy?: boolean;
  headingText: ReactNode;
  description: string;
  ctaText: string;
  imgSrc: string;
  imgAlt: string;
  className?: string;
}

function PromoBanner({
  linkTo,
  headingText,
  description,
  ctaText,
  imgSrc,
  imgAlt,
  glassy = true,
  className = "",
}: PromoBannerProps) {
  return (
    <div
      className={cn(
        "group relative flex w-full flex-col items-center justify-center p-8 pb-0 lg:flex-row lg:overflow-hidden lg:px-12",
        glassy ? "glass" : "",
        className,
      )}
    >
      <div className="z-20 flex flex-col items-center text-center lg:w-1/2 lg:items-start lg:text-left pb-8 lg:pb-0">
        <Heading margin="md" size="xxxxl" weight="bold">
          {headingText}
        </Heading>
        <Paragraph margin="lg" variant="dark">
          {description}
        </Paragraph>
        <ButtonWithIcon to={linkTo} icon={<IoArrowForwardCircle />}>
          {ctaText}
        </ButtonWithIcon>
      </div>
      <div className="relative flex w-full items-center justify-center lg:w-1/2 [perspective:1000px]">
        <img
          draggable="false"
          className="animate-float max-h-[60vh] w-full max-w-xs object-contain select-none transition-all duration-700 ease-out group-hover:scale-110 group-hover:rotate-y-12 group-hover:rotate-z-2 group-hover:brightness-110 lg:max-w-md"
          src={imgSrc}
          alt={imgAlt}
        />
      </div>
    </div>
  );
}

export default PromoBanner;
