-- refresh_tokens
CREATE INDEX ix_refresh_tokens_user_id ON terranova.refresh_tokens(user_id);

-- user_verifications
CREATE INDEX ix_user_verifications_user_id ON terranova.user_verifications(user_id);

-- user_verifications
CREATE INDEX ix_user_verifications_expires_at ON terranova.user_verifications(expires_at);

-- cart
CREATE INDEX IF NOT EXISTS ix_cart_user_id ON terranova.cart(user_id);

-- cart_item
CREATE INDEX IF NOT EXISTS ix_cart_item_cart_id ON terranova.cart_item(cart_id);
CREATE INDEX IF NOT EXISTS ix_cart_item_product_id ON terranova.cart_item(product_id);

-- product
CREATE INDEX IF NOT EXISTS ix_product_category_id ON terranova.product(category_id);
CREATE INDEX IF NOT EXISTS ix_product_sub_category_id ON terranova.product(sub_category_id);

-- product_image / variants
CREATE INDEX IF NOT EXISTS ix_product_image_product_id ON terranova.product_image(product_id);
CREATE INDEX IF NOT EXISTS ix_product_variant_product_id ON terranova.product_variant(product_id);
CREATE INDEX IF NOT EXISTS ix_product_variant_image_variant_id ON terranova.product_variant_image(product_variant_id);

-- sub_category
CREATE INDEX IF NOT EXISTS ix_sub_category_category_id ON terranova.sub_category(category_id);

-- product_sub_category (tabla puente)
CREATE INDEX IF NOT EXISTS ix_product_sub_category_product_id ON terranova.product_sub_category(product_id);
CREATE INDEX IF NOT EXISTS ix_product_sub_category_sub_category_id ON terranova.product_sub_category(sub_category_id);

-- orders / order_item
CREATE INDEX IF NOT EXISTS ix_orders_user_id ON terranova.orders(user_id);
CREATE INDEX IF NOT EXISTS ix_orders_order_status_id ON terranova.orders(order_status_id);
CREATE INDEX IF NOT EXISTS ix_orders_shipping_address_id ON terranova.orders(shipping_address_id);

CREATE INDEX IF NOT EXISTS ix_order_item_order_id ON terranova.order_item(order_id);
CREATE INDEX IF NOT EXISTS ix_order_item_product_id ON terranova.order_item(product_id);

-- invoice / invoice_item
CREATE INDEX IF NOT EXISTS ix_invoice_order_id ON terranova.invoice(order_id);
CREATE INDEX IF NOT EXISTS ix_invoice_invoice_status_id ON terranova.invoice(invoice_status_id);

CREATE INDEX IF NOT EXISTS ix_invoice_item_invoice_id ON terranova.invoice_item(invoice_id);
CREATE INDEX IF NOT EXISTS ix_invoice_item_product_id ON terranova.invoice_item(product_id);

-- payment / webhook
CREATE INDEX IF NOT EXISTS ix_payment_order_id ON terranova.payment(order_id);
CREATE INDEX IF NOT EXISTS ix_payment_payment_method_id ON terranova.payment(payment_method_id);
CREATE INDEX IF NOT EXISTS ix_payment_payment_status_id ON terranova.payment(payment_status_id);

CREATE INDEX IF NOT EXISTS ix_payment_webhook_log_payment_id ON terranova.payment_webhook_log(payment_id);

-- shipping_address / wishlist
CREATE INDEX IF NOT EXISTS ix_shipping_address_user_id ON terranova.shipping_address(user_id);
CREATE INDEX IF NOT EXISTS ix_wish_list_user_id ON terranova.wish_list(user_id);
CREATE INDEX IF NOT EXISTS ix_wish_list_item_wish_list_id ON terranova.wish_list_item(wish_list_id);
CREATE INDEX IF NOT EXISTS ix_wish_list_item_product_id ON terranova.wish_list_item(product_id);