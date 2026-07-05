-- 001_InitialSchema.sql
-- Creates the core financial domain tables for stocks, quotes, and trades.

CREATE TABLE IF NOT EXISTS stocks (
    id           BIGSERIAL     PRIMARY KEY,
    ticker       VARCHAR(10)   NOT NULL UNIQUE,
    company_name VARCHAR(200)  NOT NULL,
    sector       VARCHAR(100)  NOT NULL,
    exchange     VARCHAR(50)   NOT NULL,
    created_at   TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS quotes (
    id         BIGSERIAL     PRIMARY KEY,
    stock_id   BIGINT        NOT NULL REFERENCES stocks(id) ON DELETE CASCADE,
    price      NUMERIC(18,4) NOT NULL CHECK (price > 0),
    volume     BIGINT        NOT NULL CHECK (volume >= 0),
    quoted_at  TIMESTAMPTZ   NOT NULL
);

CREATE TABLE IF NOT EXISTS trades (
    id          BIGSERIAL     PRIMARY KEY,
    stock_id    BIGINT        NOT NULL REFERENCES stocks(id) ON DELETE CASCADE,
    side        VARCHAR(4)    NOT NULL CHECK (side IN ('BUY', 'SELL')),
    quantity    BIGINT        NOT NULL CHECK (quantity > 0),
    price       NUMERIC(18,4) NOT NULL CHECK (price > 0),
    executed_at TIMESTAMPTZ   NOT NULL,
    created_at  TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);

-- Seed a handful of well-known stocks for development and testing.
INSERT INTO stocks (ticker, company_name, sector, exchange) VALUES
    ('AAPL',  'Apple Inc.',             'Technology',          'NASDAQ'),
    ('MSFT',  'Microsoft Corporation',  'Technology',          'NASDAQ'),
    ('GOOGL', 'Alphabet Inc.',          'Communication',       'NASDAQ'),
    ('AMZN',  'Amazon.com Inc.',        'Consumer Cyclical',   'NASDAQ'),
    ('NVDA',  'NVIDIA Corporation',     'Technology',          'NASDAQ'),
    ('JPM',   'JPMorgan Chase & Co.',   'Financial Services',  'NYSE'),
    ('GS',    'Goldman Sachs Group',    'Financial Services',  'NYSE'),
    ('TSLA',  'Tesla Inc.',             'Consumer Cyclical',   'NASDAQ')
ON CONFLICT (ticker) DO NOTHING;
