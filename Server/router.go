package main

import (
	"github.com/gorilla/websocket"
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	"net/http"
	"time"
)

var upgrader = websocket.Upgrader{}

func ConnectEndpoints(e *echo.Echo) {
	upgrader.CheckOrigin = func(r *http.Request) bool {
		return true
	}
	e.GET("/", mainPage)

	e.GET("/list", List)
	e.GET("/connect/:session", Connect)

	e.GET("/stream", connectServerEcho, middleware.TimeoutWithConfig(middleware.TimeoutConfig{
		Timeout: 10 * time.Second, // 10 seconds to set up the session
	}))
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
