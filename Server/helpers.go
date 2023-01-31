package main

import (
	"github.com/google/uuid"
)

func ParseUUID(id string) (uuid.UUID, bool) {
	uid, err := uuid.Parse(id)
	return uid, err == nil
}
