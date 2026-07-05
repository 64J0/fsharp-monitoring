-- 002_AddIndexes.sql
-- Adds indexes to support common query patterns efficiently.

-- Fast ticker lookups in stock repository
CREATE INDEX IF NOT EXISTS idx_stocks_ticker ON stocks (ticker);

-- Latest quote retrieval per stock (ORDER BY quoted_at DESC)
CREATE INDEX IF NOT EXISTS idx_quotes_stock_id_quoted_at
    ON quotes (stock_id, quoted_at DESC);

-- Trade history per stock (ORDER BY executed_at DESC)
CREATE INDEX IF NOT EXISTS idx_trades_stock_id_executed_at
    ON trades (stock_id, executed_at DESC);

-- Optional: side filter for trade analytics
CREATE INDEX IF NOT EXISTS idx_trades_side ON trades (side);
