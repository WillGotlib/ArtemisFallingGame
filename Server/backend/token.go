package backend

import "github.com/google/uuid"

type Token uuid.UUID

func (t Token) UUID() uuid.UUID {
	return uuid.UUID(t)
}
func (t Token) String() string {
	return t.UUID().String()
}

func NewToken() Token {
	return Token(uuid.New())
}
