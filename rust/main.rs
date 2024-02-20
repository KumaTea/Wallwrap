#![windows_subsystem = "windows"]

use reqwest;
use std::fs;
use wallpaper;
use std::thread;
use std::path::PathBuf;
use chrono::prelude::*;
use std::time::Duration;


const PROJECT_NAME: &str = "Wallpaper";
const WALLPAPER_API: &str = "https://www.bing.com/HPImageArchive.aspx?idx=0&n=1";
const DELAY: u64 = 30; // seconds; u64 as from_secs takes

// fn get_wallpaper_path() -> PathBuf
fn get_image_folder() -> PathBuf {
    let temp_dir = std::env::temp_dir();
    temp_dir.join(PROJECT_NAME)
}

fn get_image_path(image_folder: &PathBuf) -> PathBuf {
    image_folder.join(format!("{}.jpg", Local::now().format("%Y%m%d")))
}

fn has_image(image_folder: &PathBuf, image_path: &PathBuf) -> bool {
    if !image_folder.exists() {
        fs::create_dir(&image_folder).unwrap();
    }
    let exists = image_path.exists();
    exists
}

fn get_wallpaper_url() -> String {
    let response = reqwest::blocking::get(WALLPAPER_API).unwrap();
    // find between <url> and </url>
    let body = response.text().unwrap();
    let start = body.find("<url>").unwrap() + 5;
    let end = body.find("</url>").unwrap();
    let url = format!("https://www.bing.com{}", &body[start..end]);
    url
}

fn save_wallpaper(image_path: &PathBuf) {
    let url = get_wallpaper_url();
    let response = reqwest::blocking::get(&url).unwrap();
    let bytes = response.bytes().unwrap();
    fs::write(image_path, bytes).unwrap();
}

fn set_wallpaper(image_path: &PathBuf) {
    wallpaper::set_from_path(image_path.to_str().unwrap()).unwrap();
}


fn main() {
    let image_folder = get_image_folder();
    let image_path = get_image_path(&image_folder);

    if !has_image(&image_folder, &image_path) {
        thread::sleep(Duration::from_secs(DELAY));
        save_wallpaper(&image_path);
        set_wallpaper(&image_path);
    }
}
