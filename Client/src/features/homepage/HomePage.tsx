import { IoMusicalNotes, IoShirt, IoSparkles } from "react-icons/io5";
import BentoBlockLink from "@/components/ui/BentoBlockLink";
import ProductSlider from "@/components/ui/ProductSlider";
import PromoBanner from "@/components/ui/PromoBanner";
import { getProducts } from "@/lib/utils";

function HomePage() {
  const products = getProducts("all-products");

  return (
    <>
      <div className="mb-12 flex w-full items-center justify-center lg:p-0">
        <PromoBanner
          glassy={false}
          headingText={
            <>
              New Phone, <span className="highlighted">New You.</span>
            </>
          }
          description="Upgrade your style, power up your life. Discover the latest iPhone and
          start fresh with tech that matches your ambition."
          linkTo="/products/phones"
          ctaText="Shop Now"
          imgSrc="/hand-holding-iphone.png"
          imgAlt="Hand holding an iPhone"
        />
      </div>
      <div className="flex w-full flex-col gap-6 lg:p-0">
        <BentoBlockLink
          heading="Upgrade Your Everyday – Find the Perfect Phone for You"
          paragraph="From flagship models with cutting-edge innovation to affordable options that get the job done, our collection is designed to meet every need. And with exclusive deals, fast delivery, and easy returns, upgrading your phone has never been this simple.
  Don’t wait to experience faster speeds, longer battery life, and stunning displays."
          className="hover:bg-brand-600 bg-brand-500 px-12 pt-12 lg:pt-0"
          image="/iphone.png"
          imageAlt="Stylish woman with a flower in her mouth."
          imageMargin="lg:mt-2"
          rowSpan="lg:row-span-1"
          imageClassName="w-64 lg:mr-5 mx-auto"
          linkTo="/products/phones"
        />
        <ProductSlider
          categoryName="all-products"
          products={products}
          headingText="New Arrivals"
          icon={<IoSparkles className="text-secondary-500" />}
        />
        <ProductSlider
          categoryName="clothing"
          products={getProducts("clothing")}
          headingText="Clothing"
          icon={<IoShirt className="text-secondary-500" />}
        />
        <ProductSlider
          categoryName="music-gear"
          products={getProducts("music-gear")}
          headingText="Music Gear"
          icon={<IoMusicalNotes className="text-secondary-500" />}
        />
      </div>
    </>
  );
}

export default HomePage;
