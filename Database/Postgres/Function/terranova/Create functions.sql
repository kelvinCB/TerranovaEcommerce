-- Update general field updated_at
CREATE OR REPLACE FUNCTION terranova.set_updated_at()
RETURNS trigger AS $$
BEGIN
  IF NEW IS DISTINCT FROM OLD THEN
    NEW.updated_at = now();
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;