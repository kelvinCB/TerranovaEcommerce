CREATE TABLE terranova.product_variant_image (
  id char(26) PRIMARY KEY,
  product_variant_id char(26) NOT NULL,
  image_url varchar(255) NOT NULL,
  is_main boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  is_deleted boolean NOT NULL DEFAULT FALSE
);

ALTER TABLE terranova.product_variant_image ADD CONSTRAINT fk_product_variant_image_product_variant FOREIGN KEY (product_variant_id) REFERENCES terranova.product_variant (id);
CREATE UNIQUE INDEX only_one_main_product_variant_image ON terranova.product_variant_image(product_variant_id) WHERE is_main = TRUE;