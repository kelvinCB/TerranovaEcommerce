CREATE TABLE terranova.wish_list_item (
  id char(26) PRIMARY KEY,
  wish_list_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Foreign Keys
ALTER TABLE terranova.wish_list_item ADD CONSTRAINT fk_wish_list_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);
ALTER TABLE terranova.wish_list_item ADD CONSTRAINT fk_wish_list_item_wish_list FOREIGN KEY (wish_list_id) REFERENCES terranova.wish_list (id);

-- Checks and Uniques
CREATE UNIQUE INDEX uq_wish_list_item_active ON terranova.wish_list_item (wish_list_id, product_id) WHERE is_deleted = FALSE;