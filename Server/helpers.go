package main

import (
	"fmt"
	"github.com/google/uuid"
)

func ParseID(id []byte) (uuid.UUID, bool) {
	u := fmt.Sprintf("%x", id)
	uid, err := uuid.Parse(u)
	return uid, err == nil
}
