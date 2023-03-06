package main

import (
	"github.com/gorilla/websocket"
	"github.com/labstack/echo/v4"
	"net/http"
)

var upgrader = websocket.Upgrader{}

func ConnectEndpoints(e *echo.Echo) {
	upgrader.CheckOrigin = func(r *http.Request) bool {
		return true
	}
	e.GET("/", mainPage)

	e.GET("/list", List)
	e.POST("/connect/:session", Connect)

	e.GET("/stream", connectServerEcho)
}

//func websocketWrapper(function func(ws *websocket.Conn) error) func(c echo.Context) error {
//	return func(c echo.Context) error {
//		ws, err := upgrader.Upgrade(c.Response(), c.Request(), nil)
//		if err != nil {
//			return err
//		}
//		err = function(ws)
//		if err != nil {
//			log.WithField("websocket", c.Path()).Debug(err)
//		}
//		return err
//	}
//}
