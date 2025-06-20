

-- najpierw stworz baze recznie
-- createdb -U postgres quizdb

-- odpal
-- psql -U postgres -d quizdb -f db.sql

CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username TEXT NOT NULL,
    password TEXT NOT NULL,
    createdat TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updatedat TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS quizzes (
    id SERIAL PRIMARY KEY,
    userId INT REFERENCES users(id),
    title TEXT NOT NULL,
    description TEXT,
    level TEXT,
    createdat TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updatedat TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS questions (
    id SERIAL PRIMARY KEY,
    quizid INT REFERENCES quizzes(id),
    text TEXT NOT NULL,
    questiontype TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS answers (
    id SERIAL PRIMARY KEY,
    questionid INT REFERENCES questions(id),
    text TEXT NOT NULL,
    iscorrect BOOLEAN
);

CREATE TABLE IF NOT EXISTS search_index (
    id SERIAL PRIMARY KEY,
    entity_type TEXT NOT NULL,
    entity_id INT NOT NULL,
    content TEXT NOT NULL,
    content_tsv tsvector GENERATED ALWAYS AS (to_tsvector('simple', content)) STORED
);

CREATE TABLE IF NOT EXISTS quiz_stats (
    quiz_id INT PRIMARY KEY REFERENCES quizzes(id),
    times_solved INT DEFAULT 0,
    last_solved_at TIMESTAMPTZ,
    earned_points DOUBLE PRECISION DEFAULT 0,
    total_points INT DEFAULT 0,
    average_score FLOAT GENERATED ALWAYS AS (
        CASE 
            WHEN total_points = 0 THEN 0 
            ELSE (earned_points::FLOAT / total_points) * 100 
        END
    ) STORED
);

CREATE TABLE IF NOT EXISTS user_stats (
    user_id INT PRIMARY KEY REFERENCES users(id),
    quizzes_created INT DEFAULT 0,
    quizzes_solved INT DEFAULT 0,
    earned_points DOUBLE PRECISION DEFAULT 0,
    total_points INT DEFAULT 0,
    average_score DOUBLE PRECISION GENERATED ALWAYS AS (
        CASE
            WHEN total_points = 0 THEN 0
            ELSE (earned_points / total_points) * 100
        END
    ) STORED
);


CREATE OR REPLACE VIEW quiz_list_view AS
SELECT
    q.id AS id,
    q.title AS label,
    q.description AS description,
    q.userid AS authorid,
    u.username AS username,
    COUNT(ques.id) AS question_count
FROM
    quizzes q
JOIN
    users u ON q.userid = u.id
LEFT JOIN
    questions ques ON ques.quizid = q.id
GROUP BY
    q.id, q.title, q.description, q.userid, u.username;

CREATE INDEX IF NOT EXISTS idx_search_index_tsv ON search_index USING GIN (content_tsv);

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
  NEW."updatedat" = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_search_index()
RETURNS TRIGGER AS $$
BEGIN
  IF TG_OP = 'DELETE' THEN
    DELETE FROM search_index WHERE entity_type = TG_TABLE_NAME AND entity_id = OLD.id;
  ELSE
    INSERT INTO search_index (entity_type, entity_id, content)
    VALUES (TG_TABLE_NAME, NEW.id,
        CASE
            WHEN TG_TABLE_NAME = 'users' THEN NEW.username
            WHEN TG_TABLE_NAME = 'quizzes' THEN CONCAT(NEW.title, ' ', COALESCE(NEW.description, ''))
        END
    )
    ON CONFLICT (entity_type, entity_id) DO UPDATE
    SET content = EXCLUDED.content;
  END IF;
  RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION create_quiz_stats_entry()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO quiz_stats (quiz_id)
    VALUES (NEW.id);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE UNIQUE INDEX IF NOT EXISTS search_entity_unique ON search_index(entity_type, entity_id);

CREATE TRIGGER trg_users_search_index
AFTER INSERT OR UPDATE OR DELETE ON users
FOR EACH ROW EXECUTE FUNCTION update_search_index();

CREATE TRIGGER trg_quizzes_search_index
AFTER INSERT OR UPDATE OR DELETE ON quizzes
FOR EACH ROW EXECUTE FUNCTION update_search_index();

CREATE TRIGGER set_updated_at_users
BEFORE UPDATE ON users
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER set_updated_at_quizzes
BEFORE UPDATE ON quizzes
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trg_create_quiz_stats
AFTER INSERT ON quizzes
FOR EACH ROW
EXECUTE FUNCTION create_quiz_stats_entry();



CREATE INDEX IF NOT EXISTS idx_quizzes_updatedat_desc
ON quizzes (updatedAt DESC);



INSERT INTO users (id, username, password)
VALUES (
    1,
    'admin',
    'ZehL4zUy+3hMSBKWdfnv86aCsnFowOp0Syz1juAjN8U='
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO users (id, username, password)
VALUES (
    2,
    'test',
    'n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg='
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO quizzes (id, userId, title, description, level)
VALUES (
    1,
    1,
    'Quiz Programowanie',
    'Poniżej znajdziesz łatwy quiz z programowania złożony z 10 pytań, 5 pytań jednokrotnego wyboru (tylko 1 poprawna odpowiedź), 5 pytań wielokrotnego wyboru (więcej niż 1 poprawna odpowiedź). Każde pytanie ma dokładnie 4 odpowiedzi.',
    'easy'
);

INSERT INTO questions (id, quizid, text, questiontype) VALUES
(1, 1, 'Jaki typ danych w Pythonie przechowuje wartości logiczne?', 'single'),
(2, 1, 'Co wyświetli print(2 ** 3) w Pythonie?', 'single'),
(3, 1, 'Co oznacza HTML?', 'single'),
(4, 1, 'Które rozszerzenie jest typowe dla kodu JavaScript?', 'single'),
(5, 1, 'Które z poniższych słów kluczowych służy do pętli w Pythonie?', 'single'),
(6, 1, 'Które z poniższych są językami programowania?', 'multiple'),
(7, 1, 'Co może być wynikiem działania funkcji w programie?', 'multiple'),
(8, 1, 'Jakie typy zmiennych występują w Pythonie?', 'multiple'),
(9, 1, 'Co można znaleźć w pliku .html?', 'multiple'),
(10, 1, 'Które z poniższych to pętle programistyczne?', 'multiple');

INSERT INTO answers (id, questionid, text, iscorrect) VALUES
(1, 1, 'int', false),
(2, 1, 'bool', true),
(3, 1, 'float', false),
(4, 1, 'str', false),
(5, 2, '5', false),
(6, 2, '6', false),
(7, 2, '8', true),
(8, 2, '9', false),
(9, 3, 'Hyper Text Markdown Language', false),
(10, 3, 'Highlevel Text Markup Language', false),
(11, 3, 'Hyper Transfer Markup Language', false),
(12, 3, 'Hyper Text Markup Language', true),
(13, 4, '.java', false),
(14, 4, '.js', true),
(15, 4, '.py', false),
(16, 4, '.html', false),
(17, 5, 'loop', false),
(18, 5, 'iterate', false),
(19, 5, 'for', true),
(20, 5, 'cycle', false),
(21, 6, 'Python', true),
(22, 6, 'HTML', false),
(23, 6, 'Java', true),
(24, 6, 'CSS', false),
(25, 7, 'Liczba', true),
(26, 7, 'Lista', true),
(27, 7, 'Pętla', false),
(28, 7, 'Obiekt', true),
(29, 8, 'string', true),
(30, 8, 'float', true),
(31, 8, 'char', false),
(32, 8, 'bool', true),
(33, 9, 'Znaczniki', true),
(34, 9, 'Skrypty JavaScript', true),
(35, 9, 'Komendy terminala', false),
(36, 9, 'Style CSS', true),
(37, 10, 'for', true),
(38, 10, 'do-while', true),
(39, 10, 'foreach', true),
(40, 10, 'repeat-until', true);


INSERT INTO quizzes (id, userId, title, description, level) VALUES
(2, 1, 'Quiz Ogolny Wiedzy IT', 'Quiz zawierajacy pytania z roznych dziedzin informatyki, idealny do sprawdzenia ogolnej wiedzy technicznej.', 'medium');

INSERT INTO questions (id, quizid, text, questiontype) VALUES
(11, 2, 'Jaka jest podstawowa jednostka informacji?', 'single'),
(12, 2, 'Co oznacza skrot CPU?', 'single'),
(13, 2, 'Ktory z tych systemow operacyjnych jest open source?', 'single'),
(14, 2, 'Ktore z ponizszych to przegladarki internetowe?', 'multiple'),
(15, 2, 'Co mozna zrobic za pomoca terminala?', 'multiple');

INSERT INTO answers (id, questionid, text, iscorrect) VALUES
(41, 11, 'Bajt', false),
(42, 11, 'Bit', true),
(43, 11, 'Kilobajt', false),
(44, 11, 'Znak', false),

(45, 12, 'Central Processing Unit', true),
(46, 12, 'Computer Primary Unit', false),
(47, 12, 'Central Programming Unit', false),
(48, 12, 'Control Processing Unit', false),

(49, 13, 'Windows 10', false),
(50, 13, 'macOS', false),
(51, 13, 'Linux', true),
(52, 13, 'ChromeOS', false),

(53, 14, 'Google Chrome', true),
(54, 14, 'Microsoft Word', false),
(55, 14, 'Mozilla Firefox', true),
(56, 14, 'Safari', true),

(57, 15, 'Tworzyc pliki', true),
(58, 15, 'Przegladac strony www', false),
(59, 15, 'Uruchamiac programy', true),
(60, 15, 'Zarzadzac systemem plikow', true);

INSERT INTO quizzes (id, userId, title, description, level)
VALUES
(3, 2, 'Quiz Chemia Podstawowa', 'Łatwy quiz z podstaw chemii — 5 pytań jednokrotnego wyboru i 5 pytań wielokrotnego wyboru.', 'easy')
RETURNING id;

INSERT INTO questions (id, quizid, text, questiontype) VALUES
(16, 3, 'Jaki jest symbol chemiczny wodoru?', 'single'),
(17, 3, 'Co to jest atom?', 'single'),
(18, 3, 'Które pierwiastki to metale?', 'multiple'),
(19, 3, 'Jaka jest masa molowa wody (H2O)?', 'single'),
(20, 3, 'Które związków są kwasami?', 'multiple'),
(21, 3, 'Jakie są stany skupienia materii?', 'multiple'),
(22, 3, 'Co oznacza pH roztworu?', 'single'),
(23, 3, 'Które pierwiastki są gazami w temperaturze pokojowej?', 'multiple'),
(24, 3, 'Jakie cząsteczki tworzą sole?', 'multiple'),
(25, 3, 'Co to jest reakcja chemiczna?', 'single');

INSERT INTO answers (id, questionid, text, iscorrect) VALUES
(61, 16, 'H', true),
(62, 16, 'He', false),
(63, 16, 'O', false),
(64, 16, 'C', false),
(65, 17, 'Najmniejsza cząstka pierwiastka zachowująca jego właściwości', true),
(66, 17, 'Zbiór elektronów', false),
(67, 17, 'Rodzaj molekuły', false),
(68, 17, 'Mały jon', false),
(69, 18, 'Żelazo', true),
(70, 18, 'Wodór', false),
(71, 18, 'Sód', true),
(72, 18, 'Tlen', false),
(73, 19, '18 g/mol', true),
(74, 19, '16 g/mol', false),
(75, 19, '20 g/mol', false),
(76, 19, '22 g/mol', false),
(77, 20, 'Kwas siarkowy', true),
(78, 20, 'Sól kuchenna', false),
(79, 20, 'Kwas octowy', true),
(80, 20, 'Woda', false),
(81, 21, 'Ciało stałe', true),
(82, 21, 'Gaz', true),
(83, 21, 'Plazma', false),
(84, 21, 'Ciecz', true),
(85, 22, 'Stężenie jonów wodorowych', true),
(86, 22, 'Temperatura roztworu', false),
(87, 22, 'Ciśnienie atmosferyczne', false),
(88, 22, 'Objętość roztworu', false),
(89, 23, 'Wodór', true),
(90, 23, 'Tlen', true),
(91, 23, 'Krzem', false),
(92, 23, 'Azot', true),
(93, 24, 'Kationy i aniony', true),
(94, 24, 'Atom i molekuła', false),
(95, 24, 'Elektrony', false),
(96, 24, 'Protony i neutrony', false),
(97, 25, 'Przemiana chemiczna z nowymi substancjami', true),
(98, 25, 'Przemiana fizyczna', false),
(99, 25, 'Rozpuszczanie się', false),
(100, 25, 'Zmiana temperatury', false);

INSERT INTO quiz_stats (quiz_id, times_solved, last_solved_at, earned_points, total_points)
VALUES 
(1, 25, '2025-06-19 17:30:00+02', 180, 250),
(2, 12, '2025-06-19 17:45:00+02', 87, 120),
(3, 40, '2025-06-19 18:00:00+02', 320, 400);

INSERT INTO user_stats (user_id, quizzes_created, quizzes_solved, earned_points, total_points)
VALUES
(1, 2, 10, 76.4, 100.0),
(2, 1, 6, 41.25, 60.0);

ALTER SEQUENCE answers_id_seq RESTART WITH 125;
ALTER SEQUENCE quizzes_id_seq RESTART WITH 4;
ALTER SEQUENCE users_id_seq RESTART WITH 3;
