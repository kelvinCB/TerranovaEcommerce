CREATE EXTENSION IF NOT EXISTS btree_gist;

CREATE TABLE terranova.user_verifications (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  purpose varchar(30) NOT NULL,            -- email_verify, password_reset, mfa
  code_hash varchar(255) NOT NULL,
  expires_at timestamptz NOT NULL,
  consumed_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE terranova.user_verifications ADD CONSTRAINT fk_user_verifications_user FOREIGN KEY (user_id) REFERENCES terranova.users(id);

ALTER TABLE terranova.user_verifications ADD CONSTRAINT chk_user_verifications_valid_time_window CHECK (expires_at > created_at);

ALTER TABLE terranova.user_verifications ADD CONSTRAINT chk_user_verifications_consumed_at_valid CHECK (consumed_at IS NULL OR consumed_at >= created_at);

ALTER TABLE terranova.user_verifications ADD CONSTRAINT ex_user_verifications_no_overlapping_active_window
EXCLUDE USING gist (
  user_id WITH =,
  purpose WITH =,
  tstzrange(created_at, expires_at, '[)') WITH &&
)
WHERE (consumed_at IS NULL);