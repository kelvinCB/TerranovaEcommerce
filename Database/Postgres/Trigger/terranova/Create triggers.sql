-- cart
CREATE TRIGGER trg_cart_updated_at BEFORE UPDATE ON terranova.cart FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- cart_item
CREATE TRIGGER trg_cart_item_updated_at BEFORE UPDATE ON terranova.cart_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- category
CREATE TRIGGER trg_category_updated_at BEFORE UPDATE ON terranova.category FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- invoice
CREATE TRIGGER trg_invoice_updated_at BEFORE UPDATE ON terranova.invoice FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- invoice_item
CREATE TRIGGER trg_invoice_item_updated_at BEFORE UPDATE ON terranova.invoice_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- invoice_status
CREATE TRIGGER trg_invoice_status_updated_at BEFORE UPDATE ON terranova.invoice_status FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- orders
CREATE TRIGGER trg_orders_updated_at BEFORE UPDATE ON terranova.orders FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- order_item
CREATE TRIGGER trg_order_item_updated_at BEFORE UPDATE ON terranova.order_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- order_status
CREATE TRIGGER trg_order_status_updated_at BEFORE UPDATE ON terranova.order_status FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment
CREATE TRIGGER trg_payment_updated_at BEFORE UPDATE ON terranova.payment FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment_method
CREATE TRIGGER trg_payment_method_updated_at BEFORE UPDATE ON terranova.payment_method FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment_status
CREATE TRIGGER trg_payment_status_updated_at BEFORE UPDATE ON terranova.payment_status FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- payment_webhook_log
CREATE TRIGGER trg_payment_webhook_log_updated_at BEFORE UPDATE ON terranova.payment_webhook_log FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product
CREATE TRIGGER trg_product_updated_at BEFORE UPDATE ON terranova.product FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product_image
CREATE TRIGGER trg_product_image_updated_at BEFORE UPDATE ON terranova.product_image FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product_variant
CREATE TRIGGER trg_product_variant_updated_at BEFORE UPDATE ON terranova.product_variant FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- product_variant_image
CREATE TRIGGER trg_product_variant_image_updated_at BEFORE UPDATE ON terranova.product_variant_image FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- shipping_address
CREATE TRIGGER trg_shipping_address_updated_at BEFORE UPDATE ON terranova.shipping_address FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- sub_category
CREATE TRIGGER trg_sub_category_updated_at BEFORE UPDATE ON terranova.sub_category FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- users
CREATE TRIGGER trg_users_updated_at BEFORE UPDATE ON terranova.users FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- wish_list
CREATE TRIGGER trg_wish_list_updated_at BEFORE UPDATE ON terranova.wish_list FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();

-- wish_list_item
CREATE TRIGGER trg_wish_list_item_updated_at BEFORE UPDATE ON terranova.wish_list_item FOR EACH ROW EXECUTE FUNCTION terranova.set_updated_at();